using TwoFactorAuthentication.Migrations;
using Umbraco.Core.Migrations;

namespace YubiKey2Factor.TwoFactorAuthentication.Migration
{
    public class TwoFactorPlan : MigrationPlan
    {
        public TwoFactorPlan()
            : base("TwoFactor")
        {
            From(string.Empty)
                .To<CreateTwoFactorTable>("state-1");
        }
    }
}