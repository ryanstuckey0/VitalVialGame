namespace RPGCharacterAnims.Actions
{
    public class Reload : InstantActionHandler<EmptyContext>
    {
        public override bool CanStartAction(RPGCharacterController controller)
        {
            return !controller.isRelaxed &&
                   (controller.rightWeapon == (int)Weapon.Pistol ||
                    controller.rightWeapon == (int)Weapon.Rifle ||
                    controller.rightWeapon == (int)Weapon.Shotgun ||
                    controller.rightWeapon == (int)Weapon.Pistol
                    );
        }

        protected override void _StartAction(RPGCharacterController controller, EmptyContext context)
        {
            controller.Reload();
        }
    }
}