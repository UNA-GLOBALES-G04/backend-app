using System.Text.RegularExpressions;

namespace webapp.util
{
    // extension methods for string
    public class FuzzyMatcher
    {
        private string _searchTerm;
        private string[] _searchTerms;
        private Regex _searchPattern;

        public FuzzyMatcher(string searchTerm)
        {
            _searchTerm = searchTerm.ToLower();
            _searchTerms = searchTerm.Split(' ');
            _searchPattern = new Regex(
            "(?i)(?=.*" + String.Join(")(?=.*", _searchTerms) + ")", RegexOptions.IgnoreCase);
        }
        public bool MatchFuzzy(string text)
        {
            text = text.ToLower();
            if (text == "") return true;
            if (_searchTerm == text) return true;
            if (text.Contains(_searchTerm)) return true;
            if (_searchPattern.IsMatch(text)) return true;
            return false;
        }
    }



}