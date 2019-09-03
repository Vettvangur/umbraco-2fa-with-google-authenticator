using TwoFactorAuthentication.Middleware;
using TwoFactorAuthentication.Migrations;
using Umbraco.Core;
using Umbraco.Core.Composing;
using YubiKey2Factor.TwoFactorAuthentication.Migration;

public class TwoFactorComposer : IUserComposer
{
    public void Compose(Composition composition)
    {
        // component for startup
        composition.Components().Append<TwoFactorMigrationComponent>();

        composition.Components().Append<TwoFactorEventHandler>();

        composition.Register<TwoFactorService>();
    }
}