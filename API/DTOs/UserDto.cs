namespace API.DTOs
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }

        public override bool Equals(object obj)
        {
            UserDto userDto = obj as UserDto;
            if (userDto == null)
            {
                return false;
            }
            return userDto.UserName == this.UserName
                && userDto.Token == this.Token
                && userDto.PhotoUrl == this.PhotoUrl
                && userDto.KnownAs == this.KnownAs
                && userDto.Gender == this.Gender;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserName, Token, PhotoUrl, KnownAs, Gender);
        }
    }
}
