# Linq2OData Agent Mode Design

**Status:** proposal. Nothing is implemented. The design borrows heavily from Linq2GraphQL's agent mode (`Linq2GraphQL.Client.AI`, see its `docs/design/agent-mode.md`) and adapts it to OData. See [Plan](#plan-and-risks).

## Summary

We propose an **agent mode** for Linq2OData: an opt-in package, `Linq2OData.AI`, that lets an in-process AI agent query OData services. The developer decides, in typed C# against the generated client, which entity sets the agent may query, which properties it may see, and what extra context it gets. The agent receives a scoped, simplified view of the service metadata and a small set of tools. It composes queries from OData's own query options (`$filter`, `$select`, `$expand`, `$orderby`, `$top`, `$skip`, `$count`), and the client checks every query against the scope before anything is sent. The service, called with a down-scoped on-behalf-of token, remains the security boundary.

**Why.** The same reasons as in Linq2GraphQL. Handing an agent the full `$metadata` is naive: SAP systems in particular expose many services with wide entities and cryptic names, include entity sets nobody chose, and describe themselves for developers. Hand-writing one tool per question is precise but slow and rigid. Agent mode is the middle path, and an easy on-ramp for teams that already have a Linq2OData client. Moving frequent questions to dedicated tools later counts as success.

OData suits this well: the query language is small and standardised, models know it, and the metadata describes entity sets, keys, properties and navigations precisely.

## Problem, goals and non-goals

Agents write reasonable OData queries, but only if they know the entity sets, property names and the version's syntax. V2 and V4 differ in exactly the places agents get wrong (`substringof` versus `contains`, `datetime'…'` literals, `$inlinecount` versus `$count`). Unbounded queries are a second problem specific to OData: a query without `$top` can return thousands of rows and blow the agent's context.

**Goals**

- Make it easy to expose *curated*, read-only OData access to an agent, with little orchestration code.
- Let the agent ask flexible questions within a scope the developer controls.
- Keep configuration typed against the generated client, so a metadata change that renames something breaks the build.
- Support V2 and V4 equally, with version-correct guidance and validation.
- Keep responses small: bounded page sizes and no protocol noise in what the agent reads.
- Support on-behalf-of (OBO) tokens so the agent never sees more than the current user may.

**Non-goals**

- Replacing service authorization. The client scopes what the agent *may see and query*; the service decides what it may *read*.
- Hosting, orchestration, approval workflows.
- Exposing every entity set automatically.
- **Writes in the first version**: no create, update, delete or actions. See [Writes (later)](#writes-later).
- **MCP server support** in the first version. The framework-independent core keeps it possible.

## Use cases

| Use case | Support | Notes |
| --- | --- | --- |
| **Agents in .NET applications** (interactive, background or sub-agents) on top of a Linq2OData client | Primary | Interactive agents act for the signed-in user via OBO; background agents use the app's identity with a narrow scope. |
| **SAP Gateway services** (V2) | Primary | Agents are often helpers for exploring a system. A Gateway service is cut per business area, so it has tens of entity sets, not thousands (`SapSalesQuotation.xml`: 14 entity sets, up to 56 properties each). Size comes from combining several services and from wide entities with cryptic names; see [Large services](#large-services). |
| **Single-shot "ask your data"** (one LLM call, no agent loop) | Later, via public `ScopedMetadata` | Render the scope into a prompt, validate the returned query, execute it. |
| **User-written OData with policy** (reporting, admin tools) | Later, via public `ScopedMetadata` | The validator enforces the scope on queries people write. |
| **Remote agents** (MCP, hosted tools) | Deferred | A new adapter on the core; auth and hosting need their own design. |

## First version

| First version | Later |
| --- | --- |
| Marker interface on generated clients, metadata from the embedded JSON | Dedicated tools with `o.Tool` (until then, `AIFunctionFactory.Create`) |
| Scope: `AllowAllReadOnly()` for everything read-only in one line, or allow entity sets and singletons one by one; omit, describe, instructions | Read-only functions (V4 functions, V2 `GET` function imports) |
| Renderer: Index, Outline and Full levels | Bound values (`.Bind(...)`, for example a tenant filter) |
| Validator: entity sets, keys, `$select`, `$expand`, `$orderby`, `$filter`, `$top` | V4 nested expand options (`$expand=Trips($select=Name)`) |
| Four tools, compact normalised results, page-size cap | Public `ScopedMetadata` API |
| Typed examples, `OnQuery`, token delegate | Writes, starting with dedicated tools |
| Preview package, sample, regression harness | MCP and other adapters |
| Typed clients only | Agent tools from a `$metadata` file or URL without generating a client, for exploring a service before generating one |
| Metadata used: names, types, keys, navigations, enums | Metadata annotations: labels, descriptions, filter and sort restrictions (see [Metadata annotations (later)](#metadata-annotations-later)) |

## Approaches considered

| Approach | Verdict |
| --- | --- |
| **Full `$metadata` plus a raw HTTP tool** | Rejected: uncurated, token-heavy, no validation, the agent can call anything the token reaches. |
| **One generated tool per entity set** | Rejected: dozens of tools per service, and many more once a client combines several services; models handle many similar tools poorly. |
| **Agent writes raw OData URLs** (`People?$filter=…&$select=…`) | Rejected for the first version: URL escaping is a constant source of errors, and splitting the URL apart again just to validate it adds nothing. |
| **Structured query tool**: entity set, key and each query option as separate arguments; `$filter` stays an expression | **Chosen.** Easy for the model, trivial to validate everything except `$filter`, and the client builds a correctly escaped URL. |
| **Dedicated typed tools only** | Precise but rigid. Kept as the specialisation step (see [Path to dedicated tools](#path-to-dedicated-tools)). |
| **Microsoft.OData.Core's `ODataUriParser` for validation** | Rejected: V4 only, needs an `IEdmModel` built from CSDL, and a heavy dependency. Linq2OData parses its own metadata for both versions; the validator should use the same model. |

## Architecture

| Layer | When | Contents |
| --- | --- | --- |
| **Service metadata** (generated) | At client generation | `ODataMetadata` per service, already embedded as JSON in `<Client>Helpers.cs` when `IncludeServiceMetadata` is on (the default) |
| **Agent configuration** (developer code) | When the agent is built | Allowed entity sets and singletons, omitted properties, descriptions, instructions, examples |
| **Renderer and tools** | Per agent call | The scoped metadata, rendered at a chosen detail level and served through the tools |

The metadata comes from the generator, not from a runtime `$metadata` request: no extra round-trip or credentials, it cannot disagree with the typed client, and metadata changes show up in PR diffs.

### Metadata source

The generated helper class already holds what the agent needs:

```csharp
// Generated/Client/TripPinClientHelpers.cs (exists today)
public static class TripPinServices
{
    public static ODataService TripPinService() => new() { Namespace = "...", ServicePath = "", Metadata = JsonSerializer.Deserialize<ODataMetadata>(TripPinServiceJson)! };
    public static List<ODataService> Services() => [ TripPinService() ];
    public static string TripPinServiceJson = """"{ ... }"""";
}
```

**Compile-time check.** The generated client implements a small interface from **Core**, so the generated code never depends on the AI package, and `AsAgentTools` is constrained to it:

```csharp
// Linq2OData.Core
public interface IODataMetadataSource
{
    static abstract IReadOnlyList<ODataService> Services { get; }
}

// Generated when IncludeServiceMetadata is on
public partial class TripPinClient : IODataMetadataSource
{
    static IReadOnlyList<ODataService> IODataMetadataSource.Services => TripPinServices.Services();
}
```

A client generated with `IncludeServiceMetadata = false` doesn't compile with `AsAgentTools`; a second overload takes the `$metadata` XML explicitly and parses it with `MetadataParser`.

**Annotations are not used in the first version.** The V2 parser fills `Label`, `Description`, `Filterable` and `Sortable` from SAP annotations (`sap:label`, `sap:quickinfo`, `sap:filterable`, `sap:sortable`), and the V4 parser sets `Filterable` and `Sortable` to `true` for every property. Agent mode ignores these four fields for now: the agent works from names, types, keys, navigations and enums, plus the descriptions the developer adds with `Describe`. This keeps V2 and V4 behaving the same and avoids depending on vendor-specific annotations before we know they help. See [Metadata annotations (later)](#metadata-annotations-later).

### From metadata to scoped metadata

When `AsAgentTools` runs:

1. **Read** the services' `ODataMetadata`, once per client type (cached). This relies on each entity type being stored once (see [Required changes](#required-changes-in-core-and-generator)).
2. **Keep only allowed entity sets and singletons.**
3. **Remove omitted properties and navigations.**
4. **Keep only reachable types**, following property types, navigation targets and derived types from the remaining roots. An entity type reachable only through an omitted navigation is dropped.
5. **Apply descriptions** from the configuration. Metadata descriptions and labels are not used in the first version.
6. **Services, groups and name collisions.** A client can combine several services. Each entity set remembers its service and the service's group (see [Services and groups](#services-and-groups)), so the Index can group by them, and entity set names that clash across services are prefixed with the service name; unique names stay bare.

The parsed metadata and the search index are built once per client type and shared by every user and conversation. Only the scope (steps 2–6) is applied per `AsAgentTools` call, and the result, the **scoped metadata**, is what the renderer, validator and tools run off.

### Services and groups

A client often combines several services, and on SAP several of them belong together (sales quotations, sales orders and pricing are separate Gateway services). Today a combined service has no readable identity: the generator derives its name from the first schema namespace (`ZSD_SALES_QUOTATION_SRV`, `Microsoft_OData_SampleService_Models_TripPin`), with no description and no way to say which services belong together.

**Name, description and group are set when the client is generated**, because they describe the system, not one agent's view of it:

```csharp
var request = new ClientRequest { Name = "SapClient", Namespace = "Sap" };
request.AddMetadata(quotationXml, "/sap/opu/odata/sap/ZSD_SALES_QUOTATION_SRV",
    name: "SalesQuotation", description: "Sales quotations and their items", group: "Sales");
request.AddMetadata(orderXml, "/sap/opu/odata/sap/ZSD_SALES_ORDER_SRV",
    name: "SalesOrder", description: "Sales orders, items and schedule lines", group: "Sales");
request.AddMetadata(customerXml, "/sap/opu/odata/sap/ZFI_CUSTOMER_SRV",
    name: "Customer", description: "Customer master data", group: "Finance");
```

- They are stored on `ClientMetadata` and carried into the embedded `ODataService` (`Name`, `Description`, `Group`), so the typed client, agent mode and the docs explorer all see the same structure.
- All three are optional. The name defaults to today's derived name; a service without a group is listed on its own.
- One level of grouping. Nested groups are not planned unless a real case needs them.
- Generated code is unchanged: the name, description and group are metadata only and do not affect class or member names.

**Agent mode uses them:**

- **The Index groups by group, then by service**, with the descriptions:
  ```
  Sales
    SalesQuotation  – Sales quotations and their items: QuotationSet, QuotationItemSet, …
    SalesOrder  – Sales orders, items and schedule lines: OrderSet, OrderItemSet, …
  Finance
    Customer  – Customer master data: CustomerSet, …
  ```
- **Scoping by group or service**: `o.AllowAllReadOnly(group: "Sales")` and `o.AllowServiceReadOnly("Customer")`, so a sales agent and a finance agent can share one client with different scopes. They combine with `Except`, `Omit` and the typed allows.
- **Descriptions in the agent configuration**: `o.DescribeGroup("Sales", "Quotes and orders")` and `o.DescribeService("Customer", "...")` add or replace what was set at generation.
- **`search_metadata` and `describe_entity_set` show the group and service** of each hit, so the agent knows where an entity set lives.
- **Names are checked at setup.** Groups and services are strings, not types, so `AsAgentTools` throws when one doesn't exist and lists the ones that do.

### Package layering

```
Linq2OData.AI
├─ Core (no AI framework dependency)          ├─ Adapter (thin)
│  ├─ scoped metadata                          │  └─ AsAgentTools: wraps the four
│  ├─ renderer (Index / Outline / Full)        │     operations as Microsoft.Extensions.AI
│  ├─ query validator, including $filter      │     AIFunctions
│  ├─ result shaper (V2/V4 → one format)
│  └─ the four operations as plain methods
```

The package depends on `Linq2OData.Core` and `Microsoft.Extensions.AI.Abstractions` only. The `$filter` parser is our own, so no OData library is added.

## Public API

### Quick start: everything, read-only

The easiest setup exposes every entity set and singleton, with all their properties and navigations, read-only:

```csharp
IList<AIFunction> tools = client.AsAgentTools(o => o.AllowAllReadOnly());
```

- **Everything in the client's metadata**: every entity set and singleton in every service the client was generated from, with all properties, navigations, complex types and enums.
- **Always read-only.** `AllowAllReadOnly()` gives the agent `odata_query` and the three metadata tools. It never enables writes or actions, also not once writes exist; those always need their own explicit allow.
- **The name says it.** "ReadOnly" is in the method name so a reviewer sees what is exposed without opening the docs, and so it can never be confused with a future allow that includes writes.
- **The agent is told too.** The `odata_query` description starts by saying that access is read-only, so the agent doesn't offer to change data.
- **Still scoped and validated.** Queries are validated, page sizes are capped and results are shaped exactly as with a curated scope. It differs only in what is allowed.
- **Narrow it when needed**, in the same line or later: `o.AllowAllReadOnly().Except<AuditLog>().Except(c => c.Me())`, plus `Omit`, `Describe`, `Example` and `Instructions` as usual.
- **New entity sets appear after regeneration**, visible in the diff of the embedded metadata.
- **One explicit call, not a default.** `AsAgentTools` without any allow throws, so exposing everything is always a visible line in code review.

With several services, the Index groups the entity sets by group and service, and the scope can be limited to a group or a service: `o.AllowAllReadOnly(group: "Sales")` (see [Services and groups](#services-and-groups)).

### Curated scope

```csharp
IList<AIFunction> tools = client.AsAgentTools(o =>
{
    // Entity sets and singletons (read-only)
    o.AllowEntitySet<Person>()
     .Describe("People registered in TripPin. Look people up by UserName when you have it.");
    o.AllowEntitySet<Airport>();
    o.AllowSingleton(c => c.Me());

    // Types, properties and enum values
    o.Describe<Person>(p => p.Concurrency, "Internal version number; ignore.");
    o.Describe(PersonGender.Unknown, "Not recorded.");

    // Examples for common tasks, written with the typed client
    o.Example("Trips of a person", "A person's trips with their names and budgets",
        c => c.Get<Person>(p => p.UserName = "russellwhyte")
              .Select(p => new { p.FirstName, Trips = p.Trips!.Select(t => new { t.Name, t.Budget }) }));

    // Leave out noise and sensitive data, add global context
    o.Omit<Person>(p => p.Emails);
    o.Instructions("All dates are UTC.");

    // Page size and token
    o.MaxPageSize = 50;
    o.AccessToken = ct => tokens.GetAgentTokenForUserAsync(currentUser, ct);
});

var options = new ChatOptions { Tools = [..tools] };
```

Rules:

- **Typed wherever possible.** Entity sets, properties, navigations and enum values are referenced through generated types and resolved to OData names through `[ODataEntitySet]`, `[ODataMember]` and `[ODataEnum]`. A name missing from the metadata throws at setup.
- **The scope is enforced before sending**: allowed entity sets, and properties and navigations that exist and aren't omitted, in every query option including `$filter`.
- **Allowing is read-only unless a write is added.** `AllowEntitySet<T>()` and `AllowSingleton(...)` allow queries only. When writes exist they are added explicitly on the returned options (for example `.AllowUpdate()`), never implied by an allow.
- **`AllowAllReadOnly()` and `AllowEntitySet` combine.** `AllowAllReadOnly()` sets the scope; `AllowEntitySet<T>()` after it only adds settings such as `Describe` for that entity set.
- **`Omit` removes a property or navigation from the scope**: it is not described, cannot be selected, expanded, filtered or sorted on. Filtering is covered on purpose, so the agent cannot infer an omitted value by filtering on it.
- **Descriptions overlay the metadata.** `Describe` replaces the text, with an `append` option.
- **Created per user or conversation**, so the token delegate captures who the agent acts for.
- **Examples** are typed queries rendered to OData query options without a server call (needs `ToRequest()` on the builders, see [Required changes](#required-changes-in-core-and-generator)), validated against the scope at setup.
- **`OnQuery`** sees every query the agent runs or tries to run: arguments, the URL sent, validation errors, service errors, row count and duration.
- **Fail fast.** Unknown names, or describing something omitted, throw when `AsAgentTools` runs, with a message that says what to do.

## Schema rendering and detail levels

The renderer turns the scoped metadata into compact text, close to how OData developers think about a service rather than raw CSDL XML:

```
Person  (entity set People, key UserName)  – People registered in TripPin.
  UserName: String!  FirstName: String  LastName: String  Gender: PersonGender
  AddressInfo: [Location]
  Friends → [Person]  Trips → [Trip]  Photo → Photo
```

- Navigations are shown with `→` and their multiplicity; complex types inline as their type name.
- Developer descriptions are shown next to the name they describe, which is how cryptic names (`Kunnr: String – Customer number`) become readable.
- The **conventions** block is version-specific and shown once: literal syntax, string functions, date literals, paging and the page-size cap. V2 and V4 get different text.

| Level | Contains | Typical use |
| --- | --- | --- |
| **Index** | Allowed entity sets and singletons with one-line descriptions, grouped by group and service when there are several, example names, the conventions | Always in the `odata_query` tool description |
| **Outline** | Adds properties, keys and navigations | Small services inline |
| **Full** | Adds descriptions, enum members, examples | Returned by `describe_entity_set` and `describe_type` |

The renderer is a pure function of (metadata, configuration, level), so its output is snapshot-tested.

## Agent tools

| Tool | Returns |
| --- | --- |
| `odata_query(entitySet, key?, select?, filter?, expand?, orderby?, top?, skip?, count?)` | The rows as compact JSON, or validation errors when nothing was sent. Its description starts by stating that access is read-only, followed by the Index. |
| `describe_entity_set(names[], match?)` | For each entity set or singleton: key, properties (capped on wide entities), navigations, the related types at Outline level, a **starter query** and the examples that use it. |
| `describe_type(names[])` | Full detail for entity, complex and enum types, plus the types they use at Outline level. |
| `search_metadata(text)` | Matching services, entity sets, types, properties (as `Type.Property` paths), enum members and examples, names before descriptions, each with its group and service. |

`odata_query` takes the query options as separate arguments, not a URL. `select`, `expand` and `orderby` are comma-separated lists (`orderby: "LastName desc, FirstName"`). `filter` is an OData filter expression in the service's version.

Example output of `describe_entity_set(["People"])`:

```
People  – People registered in TripPin. Look people up by UserName when you have it.
Key: UserName (String)
Properties:
  UserName: String!   FirstName: String   LastName: String   Gender: PersonGender (Male, Female, Unknown)
  AddressInfo: [Location]
Navigations: Friends → [Person], Trips → [Trip], Photo → Photo
Starter query:
  odata_query(entitySet: "People", select: "UserName,FirstName,LastName,Gender", top: 20)
Examples:
  Trips of a person  – A person's trips with their names and budgets
    odata_query(entitySet: "People", key: "'russellwhyte'", select: "FirstName", expand: "Trips($select=Name,Budget)")
```

### Results

What the agent reads must be small and the same for every version:

```json
{ "value": [ { "UserName": "russellwhyte", "FirstName": "Russell" } ], "count": 20, "hasMore": true, "nextSkip": 20 }
```

- V2 `d` / `d.results` and V4 `value` are unwrapped; a single entity returns `{ "value": { ... } }`.
- `__metadata`, `__deferred` and `@odata.*` annotations are removed.
- V2 `/Date(…)/` and string-encoded numbers are converted to ISO 8601 and JSON numbers, so the agent doesn't have to know V2 quirks.
- **Page-size cap.** `top` defaults to and is capped at `MaxPageSize` (default 50). `hasMore` and `nextSkip` come from a probe (`$top = n + 1`) or the server's `__next` / `@odata.nextLink`. The cap protects the agent's context and is stated in the conventions, so the agent asks for `count` instead of paging through everything to count.
- Service errors are returned as text (`{ "error": "...", "hint": "..." }`), never thrown into the agent loop.

### Validation

Every query is checked against the scoped metadata before it is sent, and **all** errors are returned at once, each saying what is wrong and what to do instead, with "did you mean" suggestions for near-miss names (`Firstname` → `FirstName`).

| Option | Checks |
| --- | --- |
| `entitySet` | Allowed in the scope |
| `key` | Number and types of key values; literal syntax for the version |
| `select` | Properties exist and aren't omitted; navigation paths only when expanded; required on wide entity sets (see [Large services](#large-services)) |
| `expand` | Navigations exist and aren't omitted; nested options only on V4 |
| `orderby` | Properties exist and aren't omitted; `asc`/`desc` only |
| `filter` | Parsed into an expression tree: every property path exists and isn't omitted, operand types fit, functions exist for the version (`substringof` is V2, `contains` is V4), literals use the version's syntax, `any`/`all` only on V4 collections |
| `top`, `skip` | Non-negative; `top` above the cap is lowered and reported |

The `$filter` parser is the largest single piece of work. It is a small recursive-descent parser over the OData filter grammar (logical, comparison and arithmetic operators, function calls, lambda operators, literals) with a version switch, and it is useful outside agent mode too, for validating hand-written filters.

## Large services

The main large systems are SAP. A single Gateway service is moderate in size; a system becomes large through **many services** combined in one client, and through **wide entities** (often 50–100+ properties) with cryptic names. Agents on such systems are often helpers for exploring them. The first version handles this with four measures:

- **Index grouped by group and service.** With several services, the Index lists entity sets under their group and service with descriptions, so the agent first picks the right area and service, then the entity set. Names that clash across services get the service prefix. Agents can also be scoped to one group or service. See [Services and groups](#services-and-groups).
- **`select` required on wide entity sets.** A query without `select` on an entity set with more than `o.WideEntityThreshold` properties (default 30) is rejected before sending, with a suggested `select` from the starter query. Otherwise every row carries every column and the page-size cap alone doesn't keep results small.
- **Capped `describe_entity_set`.** Wide entities list their properties up to a cap (default 60), followed by "… and 24 more, use `search_metadata` or describe with `match`". `describe_entity_set` takes an optional `match` to list only properties whose names contain a word.
- **Load once, share everywhere.** The metadata is deserialized and the search index built once per client type and cached; only the scope is applied per user. Together with storing each entity type once (see [Required changes](#required-changes-in-core-and-generator)), this keeps setup cheap even when agents are created per conversation.

Not planned, because SAP-sized services don't need them: paged listing of entity sets, path finding over the navigation graph, and name-pattern scoping (`ExceptEntitySets(n => n.StartsWith(...))`). They can be added if a service with thousands of entity sets becomes a target.

## Path to dedicated tools

1. **Start broad**: `o.AllowAllReadOnly()`, the read-only quick start, or a few `AllowEntitySet` lines.
2. **Observe**: `OnQuery` shows which questions come in and which queries run.
3. **Specialise**: frequent or sensitive questions become dedicated tools. In the first version that is `AIFunctionFactory.Create` around a typed `Query<T>()`; later `o.Tool(...)` in the same builder, with the same token, logging and result shaping. A dedicated tool is not limited by the scope, which makes it the route for sensitive entity sets.

## Security model

| Part | Responsibility |
| --- | --- |
| **Service** | Authorization: what the token may read. For SAP, the user's roles; for custom services, an agent-specific scope where possible. |
| **Token** | Identity and narrowing: the user's token exchanged (OBO) for a down-scoped agent token. |
| **Client** | Scope enforcement: the agent reaches the service only through the tools (no raw HTTP tool), every query is validated against allowed entity sets and non-omitted properties, and `odata_query` is `GET`-only. |

**The agent can't shortcut the client.** A disallowed entity set or omitted property never leaves the process, whether selected, expanded, filtered or sorted on. The service is still needed for what the client can't cover: other code using the same token, a validator bug, defense in depth.

**On-behalf-of.** The library does no token exchange. It provides a per-request seam: `client.WithAccessToken(getToken)` returns a derived client that shares the `HttpClient` and adds the token per request. `o.AccessToken` is sugar for it, and examples and dedicated tools receive the derived client. This avoids `DelegatingHandler` traps (DI scopes from `IHttpClientFactory`, `HttpContext` gone once the agent runs off the request thread).

**What remains.** Prompt injection can still make the agent read anything in scope and repeat it. The down-scoped token and the allow-list limit the damage.

## Metadata annotations (later)

Annotations could improve both what the agent is told and what the validator catches, but they are left out of the first version. If the pilots show that agents struggle with names or keep hitting filter and sort errors from the service, add them in this order:

- **Labels and descriptions** as fallbacks after `Describe`: SAP `sap:label` and `sap:quickinfo` on V2, `Org.OData.Core.V1.Description` and `Common.Label` on V4. This needs `Description` on `ODataEntityType`, labels and descriptions on `ODataNavigation` and on entity sets, and V4 parser support.
- **Filter and sort restrictions** in rendering and validation: SAP `sap:filterable` and `sap:sortable` on V2, `Org.OData.Capabilities.V1.FilterRestrictions` and `SortRestrictions` on V4. The V4 parser currently marks everything as filterable and sortable, so it must read the capabilities first.
- **Label-based suggestions** in validation errors and `search_metadata`.

Each should be switchable in the configuration, since annotation quality varies between services.

## Writes (later)

Writes are out of the first version. Decided now so the first version doesn't block them:

1. **Separate tools for writes** (`odata_create`, `odata_update`, `odata_delete`, `odata_action`), so hosts can require approval per tool. `odata_query` stays read-only by contract.
2. **Room in the builder**: `AllowEntitySet<T>()` returns options that can later take `.AllowUpdate()` and similar.
3. **`Omit` and `Describe` already work on properties**, which become input fields for writes.
4. **Dedicated write tools come first**: the developer fixes the shape, the agent fills parameters.

For when writes are built: host approval, one change per call, no automatic retries, ETag/`If-Match` for updates (the `Concurrency`-style properties exist for this), bound values, and audit through the agent identity.

## Required changes in Core and Generator

**Core (`Linq2OData.Core`), useful on their own**

- **Per-request hook and derived clients.** `ODataClient` calls `httpClient.GetAsync`/`PostAsync` directly and has no hook. Route every request through one `SendAsync` that applies `ODataClientOptions.ConfigureRequest`, and add `client.WithRequestConfiguration(hook)` and `client.WithAccessToken(getToken)` returning a client that shares the `HttpClient` and settings.
- **Stop changing the caller's `HttpClient`.** The constructor calls `httpClient.DefaultRequestHeaders.Add("Accept", ...)`, so two clients on one `HttpClient` add it twice. Set `Accept` per request in the new `SendAsync` instead.
- **Raw execution.** `ODataClient.QueryRawAsync(entitySet, key, select, expand, filter, orderby, top, skip, count)` returning the response as a `JsonNode` with `count` and next link, errors as data rather than exceptions. The result shaper in the AI package builds on it.
- **`ToRequest()` on builders** (`QueryBuilder`, `GetBuilder`, the projection builders): the entity set, key and query options without contacting the service, for rendering typed examples.
- **`IODataMetadataSource`** with a static abstract `Services` property.
- **`Name`, `Description` and `Group` on `ODataService`**, filled from the generated helper class.
- **Store each entity type once in the embedded metadata.** `ODataEntitySet.EntityType` and `ODataSingleton.EntityType` hold the full entity type object, so System.Text.Json writes every entity type again inside each entity set and singleton, in addition to `ODataSchema.EntityTypes`. This roughly doubles the embedded JSON and the client assembly, more so as clients combine several services, and after deserialization the two copies are separate objects, so values set on one (`EntityPath`, `IsSingleton`) are not reliably on the other. Mark both `EntityType` properties `[JsonIgnore]`, keep `EntityTypeName`, and relink by name after deserialization (a `Link()` method on `ODataMetadata`, called when the parser finishes and when `ODataService` loads the JSON). The generator and the agent then see one object per entity type.
- **Small cleanup**: `Converters/ODataDurationConverter.cs` is an empty file.

**Generator**

- Implement `IODataMetadataSource` on the client when `IncludeServiceMetadata` is on.
- **Service name, description and group**: optional `name`, `description` and `group` on `ClientRequest.AddMetadata` and `ClientMetadata`, written into the helper class. The name defaults to today's derived name. Generated class and member names don't change.
- Regenerate the test clients; the embedded JSON grows with the new metadata fields.

**New package `Linq2OData.AI`**

- Depends on `Linq2OData.Core` and `Microsoft.Extensions.AI.Abstractions`.
- Contains the configuration API, scoped metadata, renderer, `$filter` parser and validator, result shaper and the four tools, in two layers (see [Package layering](#package-layering)).
- Ships as a prerelease (`-preview` suffix on the shared version) until the pilots are evaluated.

## Plan and risks

**Plan**, in slices that each merge on their own:

1. **Core prerequisites**: per-request hook and derived clients, `Accept` fix, raw execution, `ToRequest()`, and storing each entity type once in the embedded metadata.
2. **Metadata source**: `IODataMetadataSource` in Core and the generator, and service name, description and group.
3. **AI package skeleton and scoped metadata**: configuration API, typed name resolution, reachability, collisions.
4. **Renderer** with snapshot tests for a V2 SAP service (`SapSalesQuotation.xml`), a V4 service (TripPin), and a client combining several services. `LargeMetadata.xml` (a Dynamics 365 service, 1,681 entity sets) is kept only as a performance test for parsing and loading, not as a design target.
5. **Validator**: options first, then the `$filter` parser for V4, then V2.
6. **Result shaper and the four tools**, then the `AsAgentTools` adapter.
7. **Sample and regression harness**: a chat loop against TripPin (V4) and a V2 demo service, plus exploration questions over a multi-service client, with an `eval` mode reporting pass rate, tool calls, rejected queries and tokens. CI runs the tool loop with a scripted chat client, no model.
8. **Pilot** on a real SAP service and decide from it.

**Risks**

- **`$filter` parsing is the long pole.** The grammar is small but V2/V4 differences and type checking take care. Mitigation: start with V4 and the common subset (comparison, logical, `contains`/`startswith`, dates), pass unknown functions through with a warning rather than rejecting them, and tighten from the pilot's logs.
- **Cryptic names without annotations.** SAP names are often unreadable, and the first version doesn't use labels. `Describe` and examples are the mitigation, and the docs must push them; annotations are the fallback if that isn't enough (see [Metadata annotations (later)](#metadata-annotations-later)).
- **Filter and sort errors from the service.** Without restrictions in the metadata, the validator can't stop a filter on a property the service doesn't support. The service error is returned with a hint, and the pilot's `OnQuery` logs show whether this is common.
- **Many services and wide entities.** An exploring agent on a client with several SAP services must pick the right service and entity set, and wide entities can still produce large results. Grouping the Index by service, requiring `select` on wide entities and capping `describe_entity_set` are the mitigations (see [Large services](#large-services)); the harness checks that the agent picks the right entity set across services.
- **Paging semantics differ between services** (server-driven paging, `$skip` support). The probe-based `hasMore` works everywhere; `nextLink` is used when present.
- **Tool conventions are moving.** Only the thin adapter depends on Microsoft.Extensions.AI.
