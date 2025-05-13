using VeriGenX.Domain.Shared;
using VeriGenX.Domain.ValueObjects;

namespace VeriGenX.Domain.Entites

{
    public record UserId(Guid userId);
    public class User
    {
        public UserId Id { get; private set; }
        public FirstName FirstName { get; private set; }
        public LastName LastName { get; private set; }

        public Email Email { get; private set; }

        private User(UserId id , FirstName firstName , LastName lastName , Email email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public static Result<User> Create(UserId id, FirstName firstName, LastName lastName, Email email)
        {
            return new User(id, firstName, lastName, email); 
        }

    }
    
}


