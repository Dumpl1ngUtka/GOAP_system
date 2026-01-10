using System.Collections.Generic;

public interface IChildPresenterRequest
{
    IEnumerable<ChildPresenterSpecification> GetChildPresenters();
}