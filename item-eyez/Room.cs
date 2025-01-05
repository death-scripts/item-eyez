namespace item_eyez
{
    public class Room
    {
        private string description;
        private string name;
        private Guid id;
        public Room(Guid id, string name, string description)
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
                ItemEyezDatabase.Instance().UpdateRoom(
                            this.Id,
                            this.Name,
                            this.Description
                        );
            }
        }
        public string Description
        {
            get { return description; }
            set
            {
                this.description = value;
                ItemEyezDatabase.Instance().UpdateRoom(
                            this.Id,
                            this.Name,
                            this.Description
                        );
            }
        }
    }
}
