namespace item_eyez
{
    public class Item : ViewModelBase
    {
        private string description;
        private string name;
        private string categories;
        private decimal value;
        private Guid id;

        public Item(Guid id, string name, string description, decimal value, string catagories)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.value = value;
            this.categories = catagories;
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
                ItemEyezDatabase.Instance().UpdateItem(
                            this.Id,
                            this.Name,
                            this.Description,
                            this.Value,
                            this.Categories
                        );
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                this.description = value;
                ItemEyezDatabase.Instance().UpdateItem(
                            this.Id,
                            this.Name,
                            this.Description,
                            this.Value,
                            this.Categories
                        );
            }
        }
        public decimal Value
        {
            get { return value; }
            set
            {
                this.value = value;
                ItemEyezDatabase.Instance().UpdateItem(
                            this.Id,
                            this.Name,
                            this.Description,
                            this.Value,
                            this.Categories
                        );
            }
        }

        public string Categories
        {
            get { return categories; }
            set
            {
                this.categories = value;
                ItemEyezDatabase.Instance().UpdateItem(
                            this.Id,
                            this.Name,
                            this.Description,
                            this.Value,
                            this.Categories
                        );
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
                if (this.ContainedIn != null)
                {
                    ItemEyezDatabase.Instance().UnassociateItemFromContainer(this.Id);
                }

                ItemEyezDatabase.Instance().SetItemsRoom(this.Id, id);
                OnPropertyChanged(nameof(StoredIn));
            }
        }
    }

}
