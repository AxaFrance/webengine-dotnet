using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.DataDriven
{
	public static class ParameterList
	{

		///<Summary>Parameter: Name of the test case </Summary>
		public static string TESTCASE { get; } = "TESTCASE";

		///<Summary>Parameter: Test environment </Summary>
		public static string Environment { get; } = "Environment";

		///<Summary>Parameter: Login </Summary>
		public static string Login { get; } = "Login";

		///<Summary>Parameter: username </Summary>
		public static string Username { get; } = "Username";

		///<Summary>Parameter: Password </Summary>
		public static string Password { get; } = "Password";

		///<Summary>Parameter: provide if search by customer id, or provide customer name </Summary>
		public static string CustomerId { get; } = "CustomerId";

		///<Summary>Parameter: provide if search by customer name. </Summary>
		public static string CustomerName { get; } = "CustomerName";

		///<Summary>Parameter: HomeLocation </Summary>
		public static string HomeLocation { get; } = "HomeLocation";

		///<Summary>Parameter: Street number of home location </Summary>
		public static string StreetNumber { get; } = "StreetNumber";

		///<Summary>Parameter: Street name of home location </Summary>
		public static string SteetName { get; } = "SteetName";

		///<Summary>Parameter: City of home location </Summary>
		public static string City { get; } = "City";

		///<Summary>Parameter: Post code of home location </Summary>
		public static string PostCode { get; } = "PostCode";

		///<Summary>Parameter: Region, State or Province of home location </Summary>
		public static string Region { get; } = "Region";

		///<Summary>Parameter: Name of the country </Summary>
		public static string Country { get; } = "Country";

		///<Summary>Parameter: HomeDetails </Summary>
		public static string HomeDetails { get; } = "HomeDetails";

		///<Summary>Parameter: Type of home, possible values: appartment, house </Summary>
		public static string HomeType { get; } = "HomeType";

		///<Summary>Parameter: Number of rooms. Possible values: 1, 2, 3, 4, 5 (more than 5 rooms, use 5) </Summary>
		public static string NumberOfRoom { get; } = "NumberOfRoom";

		///<Summary>Parameter: the surface of home in sqr meters </Summary>
		public static string HomeSurface { get; } = "HomeSurface";

		///<Summary>Parameter: Apartement only parameters </Summary>
		public static string ApartementDetails { get; } = "ApartementDetails";

		///<Summary>Parameter: Total number of floors of the building </Summary>
		public static string ApptTotalFloors { get; } = "ApptTotalFloors";

		///<Summary>Parameter: Floor number of my appartment </Summary>
		public static string ApptMyFloors { get; } = "ApptMyFloors";

		///<Summary>Parameter: If the building as an elevator </Summary>
		public static string ApptHasElevator { get; } = "ApptHasElevator";

		///<Summary>Parameter: House only parameters </Summary>
		public static string HouseDetails { get; } = "HouseDetails";

		///<Summary>Parameter: Total number of floors of the house </Summary>
		public static string HouseFloors { get; } = "HouseFloors";

		///<Summary>Parameter: the surface of backyard in sqr meters </Summary>
		public static string BackyardSurface { get; } = "BackyardSurface";

		///<Summary>Parameter: If the house has a swimming pool </Summary>
		public static string HasSwimmingPool { get; } = "HasSwimmingPool";

		///<Summary>Parameter: Antecedents </Summary>
		public static string Antecedents { get; } = "Antecedents";

		///<Summary>Parameter: If there are accidents declared </Summary>
		public static string HasAntecedents { get; } = "HasAntecedents";

		///<Summary>Parameter: Type of the accident, possible values: Vandalism, Fire, Floor </Summary>
		public static string AccidentType { get; } = "AccidentType";

		///<Summary>Parameter: Date of the accident </Summary>
		public static string AccidentDate { get; } = "AccidentDate";

		///<Summary>Parameter: If the home owner is responsible for the accident </Summary>
		public static string AccidentResponsability { get; } = "AccidentResponsability";

		///<Summary>Parameter: Offer </Summary>
		public static string Offer { get; } = "Offer";

		///<Summary>Parameter: Choose the offer, Economic, Balanced or Optimized </Summary>
		public static string OfferType { get; } = "OfferType";

		///<Summary>Parameter: If the option 24x7 is selected: yes, no </Summary>
		public static string Option24x7 { get; } = "Option24x7";

		///<Summary>Parameter: If annual payment is selected: yes, no </Summary>
		public static string OptionAnnualPayment { get; } = "OptionAnnualPayment";

		///<Summary>Parameter: If provided, verify the amount of balanced offer </Summary>
		public static string VerifyAmountBalanced { get; } = "VerifyAmountBalanced";

		///<Summary>Parameter: ValidateAndSign </Summary>
		public static string ValidateAndSign { get; } = "ValidateAndSign";

		///<Summary>Parameter: If customer will sign the contract digitally, otherwise the contract will be printed. </Summary>
		public static string DigitalSign { get; } = "DigitalSign";

	}

}
