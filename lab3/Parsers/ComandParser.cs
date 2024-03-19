namespace lab3.Parsers
{
    static class ComandParser
    {
        static public string ParseParameter(string comand, string nameComand)
        {
            if (comand.IndexOf(nameComand) == 0)
            {
                comand = comand.Remove(0, nameComand.Length);
                comand = comand.Trim();
            }
            return comand;
        }
    }
}
