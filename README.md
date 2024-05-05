# SkyNeg.EntityFramework.Migration
Update database using sql scripts at runtime

## Usage
1. Define DbContext derived from [RuntimeContext](src/SkyNeg.EntityFramework.Migration/RuntimeContext.cs)
1. Use [AddManagedDbContext\<TContext\>](src/SkyNeg.EntityFramework.Migration/DependencyInjection/ServiceCollectionExtensions.cs) extension method to configure DbContext source for SQL scripts
	1. Configure DbContext using `SetDbContextOptions(Action<DbContextOptionsBuilder> options)` method of [IManagedDbContextBuilder](src/SkyNeg.EntityFramework.Migration/Options/IManagedDbContextBuilder.cs)
	1. Configure source for scripts using provider one of the extension method from [ServiceCollectionExtensions.cs](src/SkyNeg.EntityFramework.Migration/DependencyInjection/ServiceCollectionExtensions.cs)
		1. `AddResourceScriptProvider()` - uses assembly resources as source for database scripts
		1. `AddScriptProvider<TContext>()` - uses instance of [IScriptProvider](src/SkyNeg.EntityFramework.Migration/ScriptProviders/IScriptProvider.cs) from argument
		1. `AddScriptProvider<TContext, TScriptProvider>()` - uses factory to create instance of `TScriptProvider` type derived from [IScriptProvider](src/SkyNeg.EntityFramework.Migration/ScriptProviders/IScriptProvider.cs) using `IServiceProvider`
