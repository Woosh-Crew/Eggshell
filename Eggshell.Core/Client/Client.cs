using System.Collections.Generic;

namespace Eggshell
{
    /// <summary>
    /// A Client in Eggshell is a person / the idea of someone using the
    /// application. We use client's in the context of networking, possession,
    /// authority, etc. Clients are a good way to abstract logic between people. 
    /// </summary>
    public class Client : IObject
    {
        public static List<Client> All { get; } = new();

        // Client
        // --------------------------------------------------------------------------------------- //

        public Library ClassInfo { get; }
        public Components<Client> Components { get; }

        public Client()
        {
            ClassInfo = Library.Register(this);
            Components = new(this);

            All.Add(this);
        }

        public void Delete()
        {
            Library.Unregister(this);
            All.Remove(this);
        }

        // Possession
        // --------------------------------------------------------------------------------------- //

        public List<IPossessable> Possessing { get; } = new();

        public void Possess(IPossessable possessable)
        {
            if (Possessing.Contains(possessable))
            {
                return;
            }

            possessable.OnPossess();
            Possessing.Add(possessable);
        }

        public void Unpossess(IPossessable possessable)
        {
            if (Possessing.Remove(possessable))
            {
                possessable.OnUnpossess();
            }
        }
    }
}
