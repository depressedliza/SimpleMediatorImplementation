namespace Xm.TestTask.Attr;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class HandlerType : Attribute
{
    public string Action { get; set; }

    public HandlerType(string action)
    {
        Action = action;
    }
}
