# EDApp Test

## Dotnet Core REST APIs for the online auction management

## Description

The project represents a collection of APIs for placing bids and monitoring bid activities. APIs provide the ability to persist and retrieve data using REST-full architecture. All presented artifacts are ready for production deployment using corresponding components. End-user scenarios cover following user cases:

* Authenticate user
* Refresh token
* Create product
* Place a bid
* Retrieve list of auctions
* Retrieve auction by product id

## Technical stack

* API implementation language - [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
* HTTP Server - [kestrel](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.1)
* API Framework - [ASP.NET Core](https://dotnet.microsoft.com/learn/aspnet/what-is-aspnet-core)
* Authentication - [JWT Authentication](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/)
* Infrastructure implementation language - [CloudFormation](https://aws.amazon.com/cloudformation/)
* Application life-cycle management - [ASW AKS](https://aws.amazon.com/eks/)

## Design decisions

- [x] API architecture - [REST](https://en.wikipedia.org/wiki/Representational_state_transfer)
- [x] Application design
    - API interface - [Model-View-Controller](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller)
    - Business logic - simplified Layered Architecture
        - [Service layer pattern](https://en.wikipedia.org/wiki/Service_layer_pattern)
        - [Database adapter pattern](https://en.wikipedia.org/wiki/Adapter_pattern)
- [x] Authentication
    - Refresh token redemption flow (https://docs.microsoft.com/en-us/azure/active-directory/develop/security-tokens)
        - [Tokens](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-auth-code-flow#refresh-the-access-token)
- [x] Persistence
    - In-memory collections

### APIs

#### Authenticate user

```
 * @route POST /users/authenticate
 * @param {string} userName.body.required - user name
 * @returns {} 200 - user authenticated
 * @returns {} 400 - missing request body specification
 * @returns {Error}  default - Unexpected error
```

Example:

```
POST {{baseUrl}}/users/authenticate HTTP/1.1
Content-type: application/json

{
    "userName": "test"
}
```

#### Refresh token

```
 * @route POST /users/refresh-token
 * @param {string} token.body.required - security token
 * @param {string} refreshToken.body.required - security refresh token
 * @returns {} 200 - refresh token successful
 * @returns {} 400 - missing request body specification
 * @returns {} 401 - unauthorized error
 * @returns {Error}  default - Unexpected error
```

Example

```
POST {{baseUrl}}/users/refresh-token HTTP/1.1
Content-type: application/json

{
    "token":"eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImIzYTFlMmIwLTRlNDUtNDI4OS05MWE3LTI3NDg2MWIzNjQ0NiIsImp0aSI6ImJhOTZiNTE0LTQyMzktNGY4Yy1hZGZmLTVjOGYwODZhNDVlNSIsImV4cCI6MTU5NTcyMzQ1NCwiaXNzIjoiZjgyOTE0NmQtMWJjMy00ZjE5LWFjN2MtNGZhZmY3MTRkY2VjIiwiYXVkIjoiZjgyOTE0NmQtMWJjMy00ZjE5LWFjN2MtNGZhZmY3MTRkY2VjIn0.WuWkAL8i4KJNd3u6BNkyRiVRuEVyQS4S0bTOlGJ_ph8",

    "refreshToken":"DchgKYS/VVdoDH3BTCCj0ApLWZ918GE8XCjJMdjygQ3isQZHct+vYZKjfpoGheucazfPBZmaPFYh7PXQ5SidSw=="
}
```

#### Create product

```
 * @route POST /products
 * @param {string} productName.body.required - product name
 * @returns {} 201 - product created
 * @returns {} 400 - missing request body specification
 * @returns {Error}  default - Unexpected error
```

Example

```
POST {{baseUrl}}/products HTTP/1.1
Content-type: application/json
Authorization: Bearer {{token}}

{
    "productName": "test-product 1"
}
```

#### Place a bid

```
 * @route POST /bids
 * @param {string} productId.body.required - product 
 * @param {string} price.body.required - bid price
 * @returns {} 201 - bid created
 * @returns {} 400 - missing request body specification
 * @returns {} 400 - bid with the same or greater price exists
 * @returns {} 400 - auction is closed
 * @returns {Error}  default - Unexpected error
```

Example

```
POST {{baseUrl}}/bids HTTP/1.1
Content-type: application/json
Authorization: Bearer {{token}}

{
    "productId": "fcf7cb5e-e920-4440-a23b-e43358e41c13",
    "price": 7.99
}
```

#### Retrieve list of auctions

```
 * @route GET /auctions
 * @returns {Auction[]} 200 - array of auctions
 * @returns {Error}  default - Unexpected error
```

Example

```
GET {{baseUrl}}/auctions HTTP/1.1
Authorization: Bearer {{token}}
Content-type: application/json
```

#### Retrieve auction by product id

```
 * @route GET /auctions/
 * @param {string} productId.query.required - product id
 * @returns {Auction[]} 200 - array of auctions with one element
 * @returns {Error}  default - Unexpected error
```

Example

```
GET {{baseUrl}}/auctions?productId={{productId}} HTTP/1.1
Authorization: Bearer {{token}}
Content-type: application/json

```

## Source files outline

* application - application implementation content
    - Config - application configuration files
    - Constants - global constants
    - Database - database adapter
    - Domain - application domain
        - Auction 
        - Bid
        - Product
        - User
    - Exceptions
        - BadRequestException - bad HTTP request exception (400)
        - HttpRequestException - base HTTP request exception
        - NotFoundException - not found HTTP request exception (404)
        - UnauthorizedException - unauthorized HTTP request exception (401)
    - Extensions
        - ConfigurationExtensions - IConfiguration interface extension
        - HttpContextExtensions - HttpContext extensions
        - ServiceCollectionExtensions - IServiceCollection interface extensions
    - Features - API entry points
        - Bids
            - AuctionsController - auctions API entry point
            - AuctionViewModel - auction client representation
            - BidsController - bids API entry point
        - Products
            - ProductsController - product API entry point
            - ProductService - product service
            - ProductViewModel - product client representation
        - Users
            - ApiTokenService - security token management service
            - AuthenticateService - authentication service
            - RefreshTokenService - refresh token service
            - UsersController - users API entry point
    - Middleware
        - CustomExceptionMiddleware - custom exception handling middleware
* infrastructure - infrastructure as code content
    - eks - AKS provisioning artifacts
        - aws-auth-cm.yaml - EC2 nodes provisioning script
        - eks-cluster.yaml - cluster
        - eks-cluster-network.yaml - cluster network group
        - eks-nodegroup.yaml - cluster node group
        - k8s.yaml - application container deployment script
    - user - user provisioning artifacts
        - user.yaml - creates user to be used for future infrastructure provisioning

## Local development

- Navigate to `application` folder
- Install application dependencies, build and run:

```sh
dotnet restore
dotnet build
dotnet run
```

- Navigate to http://localhost:5000
- Observe results of the calls against above mentioned APIs.

## Production deployment

- Create separate AWS user using CloudFormation template provided
- Create k8s infrastructure sing CloudFormation templates provided
    1.  Cluster network
    2.  Cluster
    3.  Node group
    4.  Authorize nodes against cluster (substitute cluster role and user arn)
- Setup CI/CD pipeline
    1.  Build docker image using Dockerfile provided
    2.  Push docker image to the private registry
    3.  Update k8s.yam file with the image registry path
    4.  Apply k8s.yam deployment
    5.  Setup DNS resolver (for example Route 53) to route traffic into the cluster (template not provided)

Note: Kubernetes network architecture provided consist of the public subnets only. This approach works well for production ready demonstrations. For more secure environments, network architecture has to be altered so nodes could be be placed into private subnets while traffic is routed via a NAT Gateways from public subnets. 

## Infrastructure

![Infrastructure architecture](./diagrams/infrastructure.png)

## Future enhancements

- [] - Separate front-end and back-end
    * Implement front-end as a separate static React web site
    * Deploy front-end into a separate s3 bucket configured for web hosting
- [] - Kubernetes
    * Introduce private subnets for nodes
    * Introduce NAT Gateways for public subnets
- [] - High performance/scalability
    * Remove In-memory collection persistence
    * Use DynamoDB as auction management store

