namespace POE_ST10434048.Part_2
{
    public abstract class UserMemory
    {
        //attributes of the user i.e. a name and a favourite topic
        public static string UserName { get; set; }
        public static string FavouriteTopic { get; set; }

        //stores the user's name
        public static void StoreName(string name)
        {
            UserName = name;
        }

        //stores the user's favourite topic
        public static void StoreFavouriteTopic(string topic)
        {
            FavouriteTopic = topic;
        }
    }
}