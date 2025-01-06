namespace item_eyez
{
    public class Container : ViewModelBase
    {
        private string description;
        private string name;
        private Guid id;

        public Container(Guid id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public Guid Id
        {
            get { return id; }
            set
            {
                this.id = value;
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                this.name = value;
                ItemEyezDatabase.Instance().UpdateContainer(this.Id, this.Name, this.Description);
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                this.description = value;
                ItemEyezDatabase.Instance().UpdateContainer(this.Id, this.Name, this.Description);
            }
        }
        public Container ContainedIn
        {
            get
            {
                return ItemEyezDatabase.Instance().GetItemsContainer(this.Id);
            }
            set
            {

                Guid? id = value == null ? null : value.Id;
                ItemEyezDatabase.Instance().SetItemsContainer(this.Id, id);
                OnPropertyChanged(nameof(ContainedIn));
            }
        }
        public Room StoredIn
        {
            get
            {

                return ItemEyezDatabase.Instance().GetItemsRoom(this.Id);
            }
            set
            {
                Guid? id = value == null ? null : value.Id;
                ItemEyezDatabase.Instance().SetItemsRoom(this.Id, id);
                OnPropertyChanged(nameof(StoredIn));
            }
        }
    }
}
