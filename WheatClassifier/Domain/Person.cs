namespace WheatClassifier.Domain;

public abstract class Person
{
    public string FirstName { get; }
    public string LastName { get; }

    protected Person(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public override string ToString() => $"{FirstName} {LastName}";
}