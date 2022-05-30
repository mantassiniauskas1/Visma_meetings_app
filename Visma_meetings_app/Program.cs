using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Meeting
{
    public string name { get; set; }
    public string responsiblePerson { get; set; }
    public string description { get; set; }
    public string category { get; set; }
    public string type { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
    public List<string> participants { get; set; }

    public Meeting(string name, string responsiblePerson, string description, string category, string type, string startDate, string endDate)
    {
        this.name = name;
        this.responsiblePerson = responsiblePerson;
        this.description = description;
        this.category = category;
        this.type = type;
        this.startDate = startDate;
        this.endDate = endDate;
        this.participants = new List<string>();
        participants.Add(responsiblePerson);
    }
    public void GetData()
    {
        Console.WriteLine("Name = {0}, responsiblePerson = {1}, description = {2}, category = {3}, type = {4}, startDate = {5}, endDate = {6}, participants = {7}",
        name, responsiblePerson, description, category, type, startDate, endDate, (string.Join(", ", participants)));
        Console.WriteLine();
    }
}

public class Program
{
    static void ReadFromFile(ref List<Meeting> meeting) //read existing meetings from data.json
    {
        string fileName = "data.json";
        string jsonString = File.ReadAllText(fileName);
        if (jsonString != "") // if data.json is empty
            meeting = JsonSerializer.Deserialize<List<Meeting>>(jsonString);
    }

    static void WriteToFile(List<Meeting> meeting) // write meetings to data.json
    {
        string fileName = "data.json";
        string jsonString = JsonSerializer.Serialize(meeting);
        File.WriteAllText(fileName, jsonString);
    }

    static void AddMeeting(ref List<Meeting> meeting) // create a new meeting
    {
        string fileName = "data.json", jsonString;
        string name, responsiblePerson, description, category, type, startDate, endDate;

        Console.WriteLine("Enter meeting name: ");
        name = Console.ReadLine();

        Console.WriteLine("Enter full name of a person responsible for the meeting: ");
        responsiblePerson = Console.ReadLine();

        Console.WriteLine("Enter description: ");
        description = Console.ReadLine();

        Console.WriteLine("Select a category(CodeMonkey/Hub/Short/TeamBuilding): ");
        category = Console.ReadLine();

        Console.WriteLine("Select a type(Live/InPerson): ");
        type = Console.ReadLine();

        Console.WriteLine("Enter start date: (e.g. 2022/06/01 08:00 AM)");
        startDate = Console.ReadLine();

        Console.WriteLine("Enter end date: (e.g. 2022/06/01 01:00 PM)");
        endDate = Console.ReadLine();

        if (new FileInfo(fileName).Length == 0)
        {
            var list = new List<Meeting>();
            list.Add(new Meeting(name, responsiblePerson, description, category, type, startDate, endDate));
            jsonString = JsonSerializer.Serialize(list);
        }
        else
        {
            var list = JsonSerializer.Deserialize<List<Meeting>>(File.ReadAllText("data.json"));
            list.Add(new Meeting(name, responsiblePerson, description, category, type, startDate, endDate));
            jsonString = JsonSerializer.Serialize(list);
        }

        File.WriteAllText("data.json", jsonString);
        ReadFromFile(ref meeting);
    }

    static bool Intersects(ref List<Meeting> meeting, string meetingName, string fullName, string startDateChosen, string endDateChosen)//check if meeting date's intersect
    {
        bool intersect = false;

        DateTime tStartA = DateTime.ParseExact(startDateChosen, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture); //string to DateTime
        DateTime tEndA = DateTime.ParseExact(endDateChosen, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

        foreach (var m in meeting)
        {
            if (m.name != meetingName && m.participants.Contains(fullName))
            {
                DateTime tStartB = DateTime.ParseExact(m.startDate, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                DateTime tEndB = DateTime.ParseExact(m.endDate, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                if (tStartA < tEndB && tStartB < tEndA) // if a meeting starts/end at the same time as another one starts/ends, they don't intersect
                    intersect = true;
            }
        }
        return intersect;
    }

    static void AddPerson(ref List<Meeting> meeting) // add a person to an existing meeting
    {
        bool personExists = false, nameExists = false;
        string startDateChosen = "", endDateChosen = "";

        Console.WriteLine("Enter full name of a person:");
        string fullName = Console.ReadLine();

        Console.WriteLine("Enter the name of the meeting you wish to add this person to:");
        string meetingName = Console.ReadLine();

        foreach (var m in meeting)
        {
            if (m.name == meetingName)
            {
                nameExists = true;
                startDateChosen = m.startDate;
                endDateChosen = m.endDate;
                if (m.participants.Contains(fullName))
                    personExists = true;
            }
        }

        if (nameExists == false)
        {
            Console.WriteLine("Meeting name doesn't exist");
            return;
        }

        if (personExists)
        {
            Console.WriteLine("Error: The same person can't be added more than once");
        }
        else
        {
            if (Intersects(ref meeting, meetingName, fullName, startDateChosen, endDateChosen))
            {
                Console.WriteLine("Error: This person is already in a meeting that intersects with this meeting's time frame");
            }
            else
            {
                foreach(var m in meeting)
                {
                    if (m.name == meetingName)
                    {
                        m.participants.Add(fullName);
                        WriteToFile(meeting);
                    }
                }
                Console.WriteLine("{0} has been added to the {1} meeting. It is scheduled to start on {2} and end on {3}", fullName, meetingName, startDateChosen, endDateChosen);
            }
        }
    }

    static void RemovePerson(ref List<Meeting> meeting) //remove a person from an existing meeting
    {
        bool nameExists = false, meetingNameExists = false;

        Console.WriteLine("Enter the name of the meeting");
        string meetingName = Console.ReadLine();

        Console.WriteLine("Enter full name of a person:");
        string fullName = Console.ReadLine();

        foreach (var m in meeting)
        {
            if (m.name == meetingName)
            {
                if (m.responsiblePerson == fullName)
                {
                    Console.WriteLine("A person responsible for the meeting cannot be removed");
                    return;
                }
                meetingNameExists = true;
                if (m.participants.Contains(fullName))
                {
                    nameExists = true;
                    m.participants.Remove(fullName);
                    WriteToFile(meeting);
                    Console.WriteLine("{0} has been removed succesesfully", fullName);
                    return;
                }
            }
        }

        if (meetingNameExists == false)
        {
            Console.WriteLine("Meeting name doesn't exist");
            return;
        }
        else if (nameExists == false)
        {
            Console.WriteLine("Person's name doesn't exist");
            return;
        }
    }

    static void DeleteMeeting(ref List<Meeting> meeting)
    {
        bool meetingNameExists = false;

        Console.WriteLine("Enter name of the meeting you wish to delete:");
        string meetingName = Console.ReadLine();

        foreach(var m in meeting)
        {
            if (m.name == meetingName)
            {
                meetingNameExists = true;
                Console.WriteLine("Are you {0}? ('y' to confirm)", m.responsiblePerson);
                string confirm1 = Console.ReadLine();
                if (confirm1.Equals("y"))
                {
                    Console.WriteLine("Are you sure you want to delete {0} meeting? ('y' to confirm)", meetingName);
                    string confirm2 = Console.ReadLine();
                    if (confirm2.Equals("y"))
                    {
                        meeting.Remove(m);
                        WriteToFile(meeting);
                        Console.WriteLine("{0} deleted successfully.", meetingName);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Task aborted");
                        return;
                    }

                }
                else
                {
                    Console.WriteLine("Task aborted");
                    return;
                }
            }
        }
          
        if (meetingNameExists == false)
        {
            Console.WriteLine("Meeting name doesn't exist");
            return;
        }

    } //delete an existing meeting

    static void FilterStr(List<Meeting> meeting, string choice, string parameter) // description, responsiblePerson, category, type filters
    {
        foreach(var m in meeting)
        {
            if (choice == "responsiblePerson" && m.responsiblePerson.Contains(parameter))
            {
                m.GetData();
            }
            else if (choice == "description" && m.description.Contains(parameter))
            {
                m.GetData();
            }
            else if (choice == "category" && m.category.Contains(parameter))
            {
                m.GetData();
            }
            else if (choice == "type" && m.type.Contains(parameter))
            {
                m.GetData();
            }
        }
    }

    static void FilterAtt(List<Meeting> meeting, string choice, string parameter) // attendees count filter
    {
        if (parameter[0] == '>')
        {
            int num = int.Parse(parameter.Substring(1));
            foreach (var m in meeting)
            {
                if (m.participants.Count > num)
                {
                    m.GetData();
                }
            }
        }
        else if (parameter[0] == '<')
        {
            int num = int.Parse(parameter.Substring(1));
            foreach (var m in meeting)
            {
                if (m.participants.Count < num)
                {
                    m.GetData();
                }
            }
        }
        if (parameter[0] == '=')
        {
            int num = int.Parse(parameter.Substring(1));
            Console.WriteLine(num);
            foreach (var m in meeting)
            {
                if (m.participants.Count == num)
                {
                    m.GetData();
                }
            }
        }
    }

    static void FilterDate(List<Meeting> meeting, string param1, string param2="") // date/dates filter
    {
        if (param2 != "") // between from and to dates filter
        { 
            DateTime inputFromDate = DateTime.ParseExact(param1, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            DateTime inputToDate = DateTime.ParseExact(param2, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

            foreach(var m in meeting)
            {
                DateTime meetingFromDate = DateTime.ParseExact(m.startDate, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                DateTime meetingToDate = DateTime.ParseExact(m.endDate, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                if (inputFromDate < meetingToDate && meetingFromDate < inputToDate)
                {
                    m.GetData();
                }
            }

        }
        else if (param2 == "") // from date filter
        {
            DateTime inputFromDate = DateTime.ParseExact(param1, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            foreach(var m in meeting)
            {
                DateTime meetingFromDate = DateTime.ParseExact(m.startDate, "yyyy/MM/dd hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
                if (meetingFromDate >= inputFromDate)
                {
                    m.GetData();
                }
            }
        }
    }

    static void Filter(List<Meeting> meeting)
    {
        Console.WriteLine("Select a filter: all, description, responsiblePerson, category, type, date, attendees");
        string choice = Console.ReadLine();
        var values = new[] { "all", "description", "responsiblePerson", "category", "type", "date", "attendees" };
        if (values.Any(choice.Contains))
        {
            if (choice == "all")
            {
                foreach(var m in meeting)
                {
                    m.GetData();
                }
            }
            else if (choice == "date")
            {
                Console.WriteLine("Select: from, between");
                choice = Console.ReadLine();

                if (choice == "from")
                {
                    Console.WriteLine("Enter a from date: (e.g. 2022/06/01 08:00 AM)");
                    string startDate = Console.ReadLine();
                    FilterDate(meeting, startDate);
                }
                else if (choice == "between")
                {
                    Console.WriteLine("Enter a from date: (e.g. 2022/06/01 08:00 AM)");
                    string startDate = Console.ReadLine();
                    Console.WriteLine("Enter a to date: (e.g. 2022/06/01 01:00 PM)");
                    string endDate = Console.ReadLine();
                    FilterDate(meeting, startDate, endDate);
                }

            }
            else if (choice == "attendees")
            {
                Console.WriteLine("Enter a parameter: (e.g. >10, <5, =7)");
                string param = Console.ReadLine();
                FilterAtt(meeting, choice, param);
            }
            else
            {
                Console.WriteLine("Enter a parameter:");
                string param = Console.ReadLine();
                FilterStr(meeting, choice, param);
            }
        }
        else
        {
            Console.WriteLine("Such filter doesn't exist");
            return;
        }


    } // general filter

    public static void Main(string[] args)
    {
        List<Meeting> meeting = new List<Meeting>();
        ReadFromFile(ref meeting); // read data from file if data exists

        Console.WriteLine("Meetings manager app \n");
        Console.WriteLine("Commands: create (Creates a new meeting)");
        Console.WriteLine("          delete (Delete an existing meeting)");
        Console.WriteLine("          add (Add a person to an existing meeting)");
        Console.WriteLine("          remove (Remove a person from an existing meeting)");
        Console.WriteLine("          list (Show all meetings)");
        
        while (true)
        {
            Console.Write("\n Command: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "create":
                    {
                        AddMeeting(ref meeting);
                        break;
                    }
                case "delete":
                    {
                        DeleteMeeting(ref meeting);
                        break;
                    }
                case "add":
                    {
                        AddPerson(ref meeting);
                        break;
                    }
                case "remove":
                    {
                        RemovePerson(ref meeting);
                        break;
                    }
                case "list":
                    {
                        Filter(meeting);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Such command doesn't exist");
                        break;
                    }
            }
        }
    }
}

