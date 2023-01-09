## Creating a client certificate like client_cert.pfx

Follow the instructions to [create a root certificate](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-7.0#create-root-ca),
then [trust it](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-7.0#install-in-the-trusted-root)
and [create a child certificate from it](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-7.0#create-child-certificate-from-root-certificate).

Since the root certificate of `client_cert.pfx` is obviously not trusted automatically by cloning this repo, the tests in `WireMockServerTests.ClientCertificate.cs` set `WireMockServerSettings.AcceptAnyClientCertificate` to `true` so that tests pass even if the device hasn't trusted the root of `client_cert.pfx`.