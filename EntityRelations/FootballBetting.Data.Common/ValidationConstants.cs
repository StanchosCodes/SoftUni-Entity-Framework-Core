namespace P02_FootballBetting.Data.Common
{
    public static class ValidationConstants
    {
        // Team validations
        public const int TeamNameMaxLength = 50;
        public const int TeamLogoUrlMaxLength = 2048;
        public const int TeamInitialsMaxLength = 4;

        // Color validations
        public const int ColorNameMaxLength = 10;

        // Town validations
        public const int TownNameMaxLength = 58;

        // Country validations
        public const int CountryNameMaxLength = 56;

        // Player validations
        public const int PlayerNameMaxLength = 100;

        // Position validations
        public const int PositionNameMaxLength = 50;

        // Game validations
        public const int GameResultMaxLength = 10;

        // User validations
        public const int UserUsernameMaxLength = 100;
        public const int UserPasswordMaxLength = 256;
        public const int UserEmailMaxLength = 255;
        public const int UserNameMaxLength = 100;
    }
}
