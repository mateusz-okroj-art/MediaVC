using System;

namespace MediaVC.Synchronizations
{
    public class ExternallyLockedRepositoryException : ApplicationException
    {
        public ExternallyLockedRepositoryException() : base("Current repository is locked externally.")
        {
        }
    }
}
