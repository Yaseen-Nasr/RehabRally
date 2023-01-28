namespace RehabRally.Web.Core.Consts
{
    public static class Errors
    {
        public const string RequiredField = "Required Field";
        public const string MaxLength = "Length cannot be more than {1} characters";
        public const string MaxMinLength = "The {0} must be at least {2} and at max {1} characters long.";
        public const string Duplicated = "Another record with the same {0} is already exists!";
        public const string DuplicatedBook = "Book with the same Title is already exists With the same Author!";
        public const string DuplicatedCategory = "Category is already exists With the same Exercise!";
        public const string NotAllowedExtension = "Only .png, .jpg, .jpeg files are allowed!";
        public const string NotAllowFutureDate = "Date can not be in the Future!";
        public const string MaxSize = "File cannot be more that 2 MB!";
        public const string ConfirmPasswordNotMatch = "The password and confirmation password do not match.";
        public const string WeekPassword = "Passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least 8 characters long";
        public const string InvalidUserName = "Username Can only contain letters or digits.";
        public const string OnlyEnglishLetters = "Only English letters are allowed.";
        public const string OnlyArabicLetters = "Only Arabic letters are allowed.";
        public const string OnlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";
        public const string DenySpecialCharacters = "Special characters are not allowed.";
        public const string InvalidRange = "{0} should be between {1} and {2}!";
        public const string InvalidPhoneNumber = "Invalid phone number!";
        public const string InvalidNationalId = "Invalid National ID!";
        public const string EmptyImege = "Please select an Image";
    }
}
