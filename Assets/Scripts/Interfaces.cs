/******************************************************************************
 * File: Interfaces.cs
 * Author: Thomas Hyman
 * Purpose: This file adds interfaces to the CloudyWaterGames namespace.
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

namespace CloudyWaterGames
{
	namespace Interfaces
	{
		/// <summary>
		/// Any implementation of this interface will be a UI element that can be
		/// highlighted.
		/// </summary>
		public interface IHighlightableUI
		{
			/// <summary>
			/// Highlights or un-highlights the UI element depending on the value of bIsHighlighted.
			/// </summary>
			/// <param name="bIsHighlighted"></param>
			void Highlight (bool bIsHighlighted);
		}

		/// <summary>
		/// Any implementation of this interface will be a UI element that can be validated.
		/// </summary>
		public interface IValidatedUI
		{
			/// <summary>
			/// Validates the UI element. Any error should be included in the ErrorMessage.
			/// </summary>
			/// <param name="ErrorMessage">Contains detailed information about validation errors.</param>
			/// <returns>True if validated, false otherwise.</returns>
			bool Validate (out string ErrorMessage);
		}

		/// <summary>
		/// Any implementation of this interface will be a struct containing trial data.
		/// </summary>
		public interface ITrialResults
		{
			/// <summary>
			/// Returns the trial results data in CSV format.
			/// </summary>
			/// <param name="numQuestions">The number of questions in the questionnaire for formatting.</param>
			/// <returns>A csv string with the trial results.</returns>
			string GetCsvData (int numQuestions);
		}
	}
}
