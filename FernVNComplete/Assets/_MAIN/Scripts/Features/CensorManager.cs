using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CensorManager
{
    // Extended dictionary to store common profanity words and their respective patterns
    private static Dictionary<string, string> badWords = new Dictionary<string, string>()
    {
        { "fuck", "f[u*][c(k|ck|c|k)]+" },
        { "shit", "s[h*][i1]t+" },
        { "bitch", "b[i1]tch" },
        { "asshole", "a[s$]{2}h[o0]le" },
        { "bastard", "b[a@]st[a@]rd" },
        { "damn", "d[a@]mn" },
        { "dick", "d[i1][c(k|c)]+" },
        { "piss", "p[i1]ss" },
        { "crap", "cr[a@]p" },
        { "hell", "h[e3]ll" },
        { "slut", "sl[u*][t7]+" },
        { "whore", "wh[o0]re" },
        { "cunt", "c[u*]nt" },
        { "nigger", "n[i1][g9]{2}[e3]r" },
        { "faggot", "f[a@][g9]{2}[o0]t" },
        { "motherfucker", "m[o0]therf[u*][c(k|ck|c|k)]+" },
        { "cock", "c[o0][c(k|ck|c|k)]+" },
        { "pussy", "p[u*][s$]{2}y" },
        { "douche", "d[o0]uch[e3]" },
        { "twat", "tw[a@]t" },
        { "prick", "pr[i1][c(k|c)]+" },
        { "ass", "a[s$]{2}" },
        { "bullshit", "bulls[h*][i1]t+" },
        { "jackass", "jacka[s$]{2}" },
        { "dildo", "d[i1]ld[o0]" },
        { "ballsack", "b[a@]lls[a@]ck" },
        { "buttplug", "buttpl[u*]g" },
        { "douchebag", "d[o0]ucheb[a@]g" },
        { "crackwhore", "cr[a@]ckwh[o0]re" },
        { "dickhead", "d[i1]ckh[e3]ad" },
        { "fucker", "f[u*]ck[e3]r" },
        { "jerkoff", "j[e3]rk[o0]ff" },
        { "spunk", "sp[u*]nk" },
        { "wanker", "w[a@]nk[e3]r" },
        { "shithead", "s[h*][i1]th[e3]ad" },
        { "pussylicker", "p[u*][s$]{2}yl[i1]ck[e3]r" },
        { "motherfucking", "m[o0]therf[u*]ck[i1]ng" },
        { "cockface", "c[o0]ckf[a@]c[e3]" },
        { "dumbass", "d[u*]mb[a@]ss" },
        { "shitfaced", "s[h*][i1]tf[a@]c[e3]d" },
        { "assfucker", "a[s$]{2}f[u*]ck[e3]r" },
        { "fuckface", "f[u*]ckf[a@]c[e3]" },
        { "twatwaffle", "tw[a@]tw[a@]ffl[e3]" },
        { "cocksucker", "c[o0]cks[u*]ck[e3]r" },
        { "douchecanoe", "d[o0]uch[e3]c[a@]n[o0][e3]" },
        { "asshat", "a[s$]{2}h[a@]t" },
        { "balllicker", "b[a@]lll[i1]ck[e3]r" },
        { "fucktard", "f[u*]ckt[a@]rd" },
        { "cumbucket", "c[u*]mb[u*]ck[e3]t" },
        { "shittiest", "s[h*][i1]tt[i1][e3]st" },
        { "whorefucker", "wh[o0]r[e3]f[u*]ck[e3]r" },
        { "cumguzzler", "c[u*]mg[u*]zzl[e3]r" },
        { "shitbag", "s[h*][i1]tb[a@]g" },
        { "pissflap", "p[i1]ssfl[a@]p" },
        { "assmunch", "a[s$]{2}m[u*]nch" },
        { "fucknugget", "f[u*]ckn[u*]gg[e3]t" }
    };

    // Whitelist of common words and names that should not be censored
    private static List<string> whitelist = new List<string>()
    {
        "Krassenstein", 
        "Scunthorpe", 
        "Penistone",
        "Dickson",
        "Cockburn",
        "Asseburg",
        "Basshole",
        "Butzbach",
        "Shitterton",
        "Dickens",
        "Coxswain",
        "Assassin",
        "Massachusetts",
        "Classic",
        "Grass",
        "Bass",
        "Passion",
        "Compassion",
        "Glass",
        "Passage",
        "Assignment"
    };

    // Method to perform censorship
    public static bool Censor(ref string text)
    {
        bool isCensored = false;

        // Check if the text matches any whitelisted words
        foreach (string name in whitelist)
        {
            if (Regex.IsMatch(text, $@"\b{name}\b", RegexOptions.IgnoreCase))
            {
                // If a whitelisted word is found, skip censorship for this text
                return isCensored;
            }
        }

        // Process bad words with advanced regex patterns that match words containing profane substrings
        foreach (var pair in badWords)
        {
            // This regex pattern matches the profane words even if they are part of another word
            string pattern = pair.Value;
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            if (regex.IsMatch(text))
            {
                text = regex.Replace(text, match => new string('*', match.Length));
                isCensored = true;
            }
        }

        return isCensored;
    }

    // Method to add new bad words dynamically
    public static void AddBadWord(string word, string pattern)
    {
        if (!badWords.ContainsKey(word))
        {
            badWords.Add(word, pattern);
        }
    }

    // Method to remove bad words dynamically
    public static void RemoveBadWord(string word)
    {
        if (badWords.ContainsKey(word))
        {
            badWords.Remove(word);
        }
    }

    // Method to add new words to the whitelist dynamically
    public static void AddWhitelistWord(string word)
    {
        if (!whitelist.Contains(word))
        {
            whitelist.Add(word);
        }
    }

    // Method to remove words from the whitelist dynamically
    public static void RemoveWhitelistWord(string word)
    {
        if (whitelist.Contains(word))
        {
            whitelist.Remove(word);
        }
    }
}
