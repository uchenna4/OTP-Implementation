using System.Drawing;
using System.Text;

namespace OTPClasses
{
    public interface IOTPGene
    {
        string generateToken();
    }
    public class OTPGene : IOTPGene 
    {
        int _lentok;
        public OTPGene(int len)
        {
            _lentok = len;
        }
        public string generateToken()
        {
            // inserted 3 sets of numeric characters so that the probability of occurence is closer
            // to that of the alphabets.
            // the digits are also strategically placed to normalize the distribution.
            const string str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz0123456789";            
            // instantiate class that generates random numbers
            Random res = new Random();            
            while (true)
            {
                StringBuilder chars = new StringBuilder("");
                int tokensiz = _lentok;
                // flags used to ensure generated string contains both alphabets and numbers
                bool alphafound = false, numfound = false;
                while (tokensiz > 0)
                {
                    // select random index in str
                    int x = res.Next(str.Length);
                    // if character at index does not already exist in random alphanumeric string
                    // append it
                    if (!(chars.ToString().Contains(str[x])))
                    {
                        chars.Append(str[x]);                        
                        --tokensiz;
                    }   
                    if (Char.IsDigit((char)str[x]))
                    {
                        if (numfound == false)
                            numfound = true;
                    }
                    else
                    {
                        if (alphafound == false)
                            alphafound = true;
                    }
                }
                // only end the process when the string contains both alphabets and digits
                if (alphafound == true && numfound == true)
                    return chars.ToString();
                else
                    continue; 
            }              
        }
    }
}