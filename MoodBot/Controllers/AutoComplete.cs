
public class Rootobject
{
    public Meta meta { get; set; }
    public Autocomplete1[] autocomplete { get; set; }
}

public class Meta
{
    public Autocomplete autocomplete { get; set; }
}

public class Autocomplete
{
    public int totalCount { get; set; }
}

public class Autocomplete1
{
    public Mainimage mainImage { get; set; }
    public int durationSeconds { get; set; }
    public string title { get; set; }
    public string url { get; set; }
    public string programId { get; set; }
}

public class Mainimage
{
    public string url { get; set; }
}

