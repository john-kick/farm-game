namespace FarmGame.Scripts.Controls.Interactions
{
    public interface IInteractable
    {
        public Interaction PrimaryInteraction();
        public Interaction SecondaryInteraction();
        public Interaction TertiaryInteraction();
    }
}