using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Converters
{
    /// <summary>
    /// 文字を全角に変換します
    /// </summary>
    public  static class StringWideConverter
    {
        public static string ToWide(this string chars)
        {
            var newChars = new char[chars.Length];
            int length = 0;
            for (int i = 0; i < chars.Length; i++, length++)
            {
                if (0x0021 <= chars[i] && chars[i] <= 0x007d)
                {
                    newChars[length] = (char)(chars[i] + (0xff00 - 0x20));
                }
                else if (chars[i] == ' ')
                {
                    newChars[length] = '　';
                }
                else if (chars[i] == 0x002D
                         || chars[i] == 0x2010
                         || chars[i] == 0x2011
                         || chars[i] == 0x2013
                         || chars[i] == 0x2014
                         || chars[i] == 0x2015
                         || chars[i] == 0x2212
                         || chars[i] == 0xFF70
                         || chars[i] == 0xFF0D)
                {
                    newChars[length] = (char)0x30FC;
                }
                else if (0xFF66 <= chars[i] && chars[i] <= 0xFF9D)
                {
                    if (i < (chars.Length - 1))
                    {
                        if (chars[i + 1] == 0xFF9E)
                        {
                            if (HalfKanaToWideKana.ContainsKey((chars[i], (char)0xFF9E)))
                            {
                                newChars[length] = HalfKanaToWideKana[(chars[i], (char)0xFF9E)];
                                i++;
                                continue;
                            }
                        }
                        else if (chars[i + 1] == 0xFF9F)
                        {
                            if (HalfKanaToWideKana.ContainsKey((chars[i], (char)0xFF9F)))
                            {
                                newChars[length] = HalfKanaToWideKana[(chars[i], (char)0xFF9F)];
                                i++;
                                continue;
                            }
                        }
                    }

                    if (HalfKanaToWideKana.ContainsKey((chars[i], ' ')))
                    {
                        newChars[length] = HalfKanaToWideKana[(chars[i], ' ')];
                        continue;
                    }

                    newChars[length] = chars[i];
                }
                else if (0xFF61 <= chars[i] && chars[i] <= 0xFF65)
                {
                    if (chars[i] == 0xFF61)
                    {
                        newChars[length] = (char)0x3002;
                    }
                    else if (chars[i] == 0xFF62)
                    {
                        newChars[length] = (char)0x300C;
                    }
                    else if (chars[i] == 0xFF63)
                    {
                        newChars[length] = (char)0x300D;
                    }
                    else if (chars[i] == 0xFF64)
                    {
                        newChars[length] = (char)0x3001;
                    }
                    else if (chars[i] == 0xFF65)
                    {
                        newChars[length] = (char)0x30FB;
                    }
                    else
                    {
                        newChars[length] = chars[i];
                    }
                }
                else
                {
                    newChars[length] = chars[i];
                }
            }
            return new string(newChars, 0, length);
        }

        private static readonly Dictionary<(char, char), char> HalfKanaToWideKana = new Dictionary<(char, char), char>
        {
            { ('ｱ', ' '), 'ア'}
            ,{ ('ｲ', ' '), 'イ'}
            ,{ ('ｳ', ' '), 'ウ'}
            ,{ ('ｴ', ' '), 'エ'}
            ,{ ('ｵ', ' '), 'オ'}
            ,{ ('ｧ', ' '), 'ァ'}
            ,{ ('ｨ', ' '), 'ィ'}
            ,{ ('ｩ', ' '), 'ゥ'}
            ,{ ('ｪ', ' '), 'ェ'}
            ,{ ('ｫ', ' '), 'ォ'}
            ,{ ('ｳ' , 'ﾞ'), 'ヴ'}
            ,{ ('ｶ', ' '), 'カ'}
            ,{ ('ｷ', ' '), 'キ'}
            ,{ ('ｸ', ' '), 'ク'}
            ,{ ('ｹ', ' '), 'ケ'}
            ,{ ('ｺ', ' '), 'コ'}
            ,{ ('ｶ' , 'ﾞ'), 'ガ'}
            ,{ ('ｷ' , 'ﾞ'), 'ギ'}
            ,{ ('ｸ' , 'ﾞ'), 'グ'}
            ,{ ('ｹ' , 'ﾞ'), 'ゲ'}
            ,{ ('ｺ' , 'ﾞ'), 'ゴ'}
            ,{ ('ｻ', ' '), 'サ'}
            ,{ ('ｼ', ' '), 'シ'}
            ,{ ('ｽ', ' '), 'ス'}
            ,{ ('ｾ', ' '), 'セ'}
            ,{ ('ｿ', ' '), 'ソ'}
            ,{ ('ｻ' , 'ﾞ'), 'ザ'}
            ,{ ('ｼ' , 'ﾞ'), 'ジ'}
            ,{ ('ｽ' , 'ﾞ'), 'ズ'}
            ,{ ('ｾ' , 'ﾞ'), 'ゼ'}
            ,{ ('ｿ' , 'ﾞ'), 'ゾ'}
            ,{ ('ﾀ', ' '), 'タ'}
            ,{ ('ﾁ', ' '), 'チ'}
            ,{ ('ﾂ', ' '), 'ツ'}
            ,{ ('ﾃ', ' '), 'テ'}
            ,{ ('ﾄ', ' '), 'ト'}
            ,{ ('ﾀ' , 'ﾞ'), 'ダ'}
            ,{ ('ﾁ' , 'ﾞ'), 'ヂ'}
            ,{ ('ﾂ' , 'ﾞ'), 'ヅ'}
            ,{ ('ﾃ' , 'ﾞ'), 'デ'}
            ,{ ('ﾄ' , 'ﾞ'), 'ド'}
            ,{ ('ﾅ', ' '), 'ナ'}
            ,{ ('ﾆ', ' '), 'ニ'}
            ,{ ('ﾇ', ' '), 'ヌ'}
            ,{ ('ﾈ', ' '), 'ネ'}
            ,{ ('ﾉ', ' '), 'ノ'}
            ,{ ('ﾊ', ' '), 'ハ'}
            ,{ ('ﾋ', ' '), 'ヒ'}
            ,{ ('ﾌ', ' '), 'フ'}
            ,{ ('ﾍ', ' '), 'ヘ'}
            ,{ ('ﾎ', ' '), 'ホ'}
            ,{ ('ﾊ' , 'ﾞ'), 'バ'}
            ,{ ('ﾋ' , 'ﾞ'), 'ビ'}
            ,{ ('ﾌ' , 'ﾞ'), 'ブ'}
            ,{ ('ﾍ' , 'ﾞ'), 'ベ'}
            ,{ ('ﾎ' , 'ﾞ'), 'ボ'}
            ,{ ('ﾊ' , 'ﾟ'), 'パ'}
            ,{ ('ﾋ' , 'ﾟ'), 'ピ'}
            ,{ ('ﾌ' , 'ﾟ'), 'プ'}
            ,{ ('ﾍ' , 'ﾟ'), 'ペ'}
            ,{ ('ﾎ' , 'ﾟ'), 'ポ'}
            ,{ ('ﾏ', ' '), 'マ'}
            ,{ ('ﾐ', ' '), 'ミ'}
            ,{ ('ﾑ', ' '), 'ム'}
            ,{ ('ﾒ', ' '), 'メ'}
            ,{ ('ﾓ', ' '), 'モ'}
            ,{ ('ﾔ', ' '), 'ヤ'}
            ,{ ('ﾕ', ' '), 'ユ'}
            ,{ ('ﾖ', ' '), 'ヨ'}
            ,{ ('ｬ', ' '), 'ャ'}
            ,{ ('ｭ', ' '), 'ュ'}
            ,{ ('ｮ', ' '), 'ョ'}
            ,{ ('ｯ', ' '), 'ッ'}
            ,{ ('ﾗ', ' '), 'ラ'}
            ,{ ('ﾘ', ' '), 'リ'}
            ,{ ('ﾙ', ' '), 'ル'}
            ,{ ('ﾚ', ' '), 'レ'}
            ,{ ('ﾛ', ' '), 'ロ'}
            ,{ ('ﾜ', ' '), 'ワ'}
            ,{ ('ﾝ', ' '), 'ン'}
            ,{ ('ｦ', ' '), 'ヲ'}
        };
    }
}
