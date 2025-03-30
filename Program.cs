using Microsoft.AspNetCore.HttpOverrides;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
}
await app.BootUmbracoAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

#if (UseHttpsRedirect)
app.UseHttpsRedirection();
#endif

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
