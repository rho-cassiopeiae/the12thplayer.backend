# The12thPlayer

The backend for **The12thPlayer** mobile app. The goal of the application is to bring football fans closer together by providing a friendly, feature-rich platform where they can interact with each other in many different ways. The app does all of the usual football app things like live match events, lineups, and stats, but its main focus is on fans interconnecting with other fellow fans in order to build a strong and united community.

You can chat about a game in a discussion room, watch post-match fan video reactions and post your own, rate players' and managers' performances, predict match outcomes, and more. For more details on the available and planned functionality go to [this](https://github.com/rho-cassiopeiae/the12thplayer "this") repository, where the mobile app itself is hosted.

<p float="left">
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/1.png" width="24%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/2.png" width="24%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/5.png" width="24%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/7.png" width="24%" />
</p>

<details>
<summary>More screenshots</summary>

<p float="left">
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/8.png" width="32%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/4.png" width="32%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/12.png" width="32%" />
</p>
<p float="left">
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/9.png" width="32%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/6.png" width="32%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/13.png" width="32%" />
</p>
<p float="left">
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/10.png" width="32%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/11.png" width="32%" />
    <img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/3.png" width="32%" />
</p>

</details>

## Architecture

The application is built on the microservice architecture, i.e., following the principals of domain ownership and data sovereignty.

All communications between services go through a message broker (RabbitMQ in dev) abstracted behind MassTransit.

The Api/Host projects (layers) in every service are entrypoints to the services (i.e., they contain MVC controllers and/or SignalR hubs, and/or MassTransit consumers). The entrypoint layer is very thin and doesn't contain any business logic. Instead, all of the business logic lives in the Application layer. Having a thin entrypoint layer allows for a simplified integration testing (no need for `WebApplicationFactory`, `TestServer`, and `HttpClient`) and also enables us to replace the built-in ASP.NET 5 filter pipeline with our own application (Mediator) pipeline, in order to avoid some of the inconsistencies in the filter pipeline behavior (in particular, the way authorization is handled in MVC and SignalR).

The Application layer manipulates the domain. It mostly consists of mediator behaviors and command and query handlers. This layer is generally coded against interfaces without any knowledge of the concrete implementations.

The Domain layer contains aggregates and some base interfaces and classes. Other DDD patterns (such as repository, domain and integration events, etc.) are used where appropriate.

The Infrastructure layer takes care of external concerns, such as database, cache, third-party services, etc. All but one services that have a database use PostgreSQL and EF Core. But Feed service uses CockroachDB and ADO.NET (Npgsql).

<img src="https://raw.githubusercontent.com/rho-cassiopeiae/the12thplayer/dev/.github/images/architecture.png" />

Worker service is a job scheduling/execution service which uses Quartz.NET underneath. It receives commands to schedule/run a certain job (or a series of jobs) from Admin and does it. For example, a command to collect initial data about a certain team (squad, finished fixtures, etc.) and transfer it to the interested services, so that they can populate their databases. Or a command to periodically update a team's upcoming fixtures (since they get rescheduled all the time).

FileHostingGateway is a service that reads files from disk and uploads them to S3/Vimeo/whatever. For example, when a user uploads a post-match video reaction, Livescore service receives the file in chunks (to not fill up the available RAM) and performs some basic validation (extension, signature, file size). It writes the file to disk and then sends a message containing the path to FileHostingGateway.

The idea in production is to use a shared network drive (like AWS EFS) to make the same drive available to all interested service instances (like Livescore and FileHostingGateway), so that they can share files between themselves. Meaning, Livescore receives the video and saves it to the drive, FileHostingGateway picks it up and uploads to Vimeo. Vimeo runs its own more sophisticated checks, transcodes the file, and returns a url.

In development we use a shared Docker volume to emulate a shared network drive.