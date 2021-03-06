﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BkTreeSpellChecker
{
    // results returned when a word is checked for spelling correction
    public sealed class SpellCheckResult
    {
        #region properties & variables

        public bool Found { get; set; }
        public Dictionary<double, List<string>> Suggestions { get; set; } // TODO -> Find a nice way to do this suggestion thingy  (too bulky with a list for each distance)

        private const int TotalSuggestions = 10;

        private readonly double _changeRate;

        private int ErrorMargin { get; set; }
        private string Word { get; set; }
        private readonly string[] _wordsSuggested;

        #endregion

        #region ctor

        public SpellCheckResult(double changeRate)
        {
            Suggestions = new Dictionary<double, List<string>>();
            _wordsSuggested = new string[10];
            _changeRate = changeRate;
        }

        #endregion

        #region public methods

        public void SetObject(int error, string word = null)
        {
            ResetObject(true);
            Word = word;
            ErrorMargin = error;
        }

        public string GetResultText()
        {
            GetResultArray();
            var result = $"Error margin {ErrorMargin} & word '{Word}' is " + (Found ? "correct" : $"incorrect - top 10 suggestions: {string.Join(",", _wordsSuggested.Select(p => p))}");
            return result;
        }

        public string[] GetResultCopy()
        {
            if (Found)
            {
                return null;
            }

            GetResultArray();
            var result = new string[TotalSuggestions];
            Array.Copy(_wordsSuggested, result, TotalSuggestions);
            return result;
        }

        private void GetResultArray()
        {
            if (Found)
            {
                return;
            }

            var i = _changeRate;
            var count = 0;

            // this will always iterate at most as the value of TotalSuggestions
            while (true)
            {
                if (Suggestions.ContainsKey(i))
                {
                    var list = Suggestions[i];
                    foreach (var t in list)
                    {
                        if (count == TotalSuggestions)
                        {
                            break;
                        }

                        _wordsSuggested[count] = t;
                        count++;
                    }
                }

                if (i >= ErrorMargin)
                {
                    break;
                }

                i += _changeRate;
            }
        }

        // resets object to default value
        public void ResetObject(bool resetMargin)
        {
            if (resetMargin)
            {
                ErrorMargin = 0;
            }

            Suggestions.Clear();
            Found = false;
            Word = string.Empty;
            Array.Clear(_wordsSuggested, 0, _wordsSuggested.Length);
        }

        #endregion
    }
}