namespace EcoClean.API.Helpers;

public static class BMICalculator
{
    public static double Calculate(double weightKg, double heightCm)
        => weightKg / Math.Pow(heightCm / 100.0, 2);

    public static string Classify(double bmi) => bmi switch
    {
        < 18.5 => "Underweight",
        < 25.0 => "Normal",
        < 30.0 => "Overweight",
        _      => "Obese"
    };
}

public static class TDEECalculator
{
    // Mifflin-St Jeor equation
    public static double BMR(double weight, double height, int age, string gender)
    {
        var bmr = (10.0 * weight) + (6.25 * height) - (5.0 * age);
        return gender.ToLower() == "female" ? bmr - 161.0 : bmr + 5.0;
    }

    public static double TDEE(double bmr, string activityLevel) => activityLevel switch
    {
        "Sedentary"        => bmr * 1.2,
        "LightlyActive"    => bmr * 1.375,
        "ModeratelyActive" => bmr * 1.55,
        "VeryActive"       => bmr * 1.725,
        "ExtraActive"      => bmr * 1.9,
        _                  => bmr * 1.2
    };

    public static int DailyCalorieGoal(double tdee, string goal) => goal switch
    {
        "LoseWeight" => (int)(tdee - 500),
        "GainWeight" => (int)(tdee + 300),
        _            => (int)tdee
    };
}
