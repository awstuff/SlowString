using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Awstuff.SlowString {

    public sealed class SlowString : IComparable, IComparable<SlowString>, ICloneable, IEnumerable<char>, IEquatable<SlowString> {

        public static readonly SlowString EmptySlowString = new SlowString();

        private char[] chars;


        private SlowString () {
            this.chars = new char[0];
        }


        public SlowString (char[] source) : this(source, 0) { }


        public SlowString (char[] source, int startIndex) : this(source, startIndex, source.Length - startIndex) { }


        public SlowString (char[] source, int startIndex, int length) {
            this.chars = new char[length];

            Array.Copy(source, startIndex, this.chars, 0, length);
        }


        private SlowString (char[] source, object doNotCopy) {
            this.chars = source;
        }


        public SlowString (char source, int count) {
            var arr = new char[count];

            for (var i = 0; i < count; i++) {
                arr[i] = source;
            }

            this.chars = arr;
        }


        public SlowString (StringBuilder source) : this(source.ToString()) { }


        public SlowString (string source) : this(source, 0) { }


        public SlowString (string source, int startIndex) : this(source, startIndex, source.Length - startIndex) { }


        public SlowString (string source, int startIndex, int length) : this(source.ToCharArray(startIndex, length)) { }


        public static implicit operator SlowString (string source) {
            return new SlowString(source);
        }


        public static bool operator == (SlowString first, SlowString second) {
            return Equals(first, second);
        }


        public static bool operator != (SlowString first, SlowString second) {
            return !(first == second);
        }


        public static SlowString operator + (SlowString first, SlowString second) {
            return Concat(first, second);
        }


        public char this [int index] {
            get {
                this.CheckIndex(index);
                return this.chars[index];
            }
            set {
                this.CheckIndex(index);
                this.chars[index] = value;
            }
        }


        public int Length {
            get { return this.chars.Length; }
        }


        private void CheckIndex (int index) {
            if (index < 0 || index > this.chars.Length - 1) {
                throw new IndexOutOfRangeException();
            }
        }


        public static SlowString Concat (SlowString first, SlowString second) {
            if (ReferenceEquals(first, null)) {
                return second;
            }

            return first.Concat(second);
        }


        public SlowString Concat (SlowString other) {
            if (ReferenceEquals(other, null)) {
                return this;
            }

            var oldLength = this.Length;
            var newLength = this.Length + other.Length;

            var arr = this.chars;

            Array.Resize(ref arr, newLength);

            for (int i = oldLength, j = 0; i < newLength; i++, j++) {
                arr[i] = other[j];
            }

            return new SlowString(arr, null);
        }


        public static SlowString Join<T> (SlowString separator, IEnumerable<T> values) {
            if (separator == null) {
                separator = EmptySlowString;
            }

            using (var enumerator = values.GetEnumerator()) {
                if (!enumerator.MoveNext()) {
                    return EmptySlowString;
                }

                var separatorAsString = separator.ToString();

                var builder = new StringBuilder();

                var current = enumerator.Current;

                if (current != null) {
                    builder.Append(current);
                }

                while (enumerator.MoveNext()) {
                    current = enumerator.Current;
                    builder.Append(separatorAsString);

                    if (current != null) {
                        builder.Append(current);
                    }
                }

                return new SlowString(builder.ToString());
            }
        }


        public static SlowString Join<T> (SlowString separator, T[] values) {
            if (separator == null) {
                separator = EmptySlowString;
            }

            var separatorAsString = separator.ToString();

            var builder = new StringBuilder();

            var isFirstValue = true;

            for (var i = 0; i < values.Length; i++) {
                if (isFirstValue) {
                    isFirstValue = false;
                } else {
                    builder.Append(separatorAsString);
                }

                builder.Append(values[i]);
            }

            return new SlowString(builder.ToString());
        }


        public SlowString ToLowerCase () {
            return new SlowString(this.ToLowerCase(0), null);
        }


        private char[] ToLowerCase (int startIndex) {
            var length = this.chars.Length;

            var arr = new char[length];

            for (var i = startIndex; i < length; i++) {
                arr[i] = char.ToLower(this.chars[i]);
            }

            return arr;
        }


        public SlowString ToUpperCase () {
            return new SlowString(this.ToUpperCase(0), null);
        }


        private char[] ToUpperCase (int startIndex) {
            var length = this.chars.Length;

            var arr = new char[length];

            for (var i = startIndex; i < length; i++) {
                arr[i] = char.ToUpper(this.chars[i]);
            }

            return arr;
        }


        public SlowString Capitalize () {
            if (this.Length < 1) {
                return this;
            }

            var arr = this.CloneChars();

            arr[0] = char.ToUpper(this.chars[0]);

            return new SlowString(arr, null);
        }


        public SlowString CapitalizeWords () {
            var length = this.chars.Length;

            var arr = new char[length];

            var lastCharWasWhitespace = true;

            for (var i = 0; i < length; i++) {
                var currentChar = this.chars[i];

                if (lastCharWasWhitespace) {
                    arr[i] = char.ToUpper(this.chars[i]);
                    lastCharWasWhitespace = false;
                } else {
                    arr[i] = this.chars[i];
                }

                if (char.IsWhiteSpace(currentChar)) {
                    lastCharWasWhitespace = true;
                }
            }

            return new SlowString(arr, null);
        }


        public static int Compare (SlowString first, SlowString second) {
            if (ReferenceEquals(first, second)) {
                return 0;
            }

            if (ReferenceEquals(first, null)) {
                return -1;
            }

            if (ReferenceEquals(second, null)) {
                return 1;
            }

            return first.CompareTo(second);
        }


        public int CompareTo (object other) {
            if (!(other is SlowString)) {
                throw new ArgumentException("SlowString can only be compared to another SlowString");
            }

            return this.CompareTo((SlowString) other);
        }


        public int CompareTo (SlowString other) {
            if (ReferenceEquals(other, null)) {
                return 1;
            }

            var firstChars = this.chars;
            var firstCharsLength = firstChars.Length;
            var secondChars = other.chars;
            var secondCharsLength = secondChars.Length;

            for (var i = 0; i < firstCharsLength; i++) {
                if (i == secondCharsLength) {
                    return 1;
                }

                var charComparison = firstChars[i].CompareTo(secondChars[i]);

                if (charComparison != 0) {
                    return charComparison;
                }
            }

            return firstCharsLength - secondCharsLength;
        }


        public object Clone () {
            return this;
        }


        public TypeCode GetTypeCode () {
            return TypeCode.Object;
        }


        public static bool IsNullOrEmpty (SlowString input) {
            return input == null || input.Length == 0;
        }


        private static bool IsNullOrEmpty (char[] input) {
            return input == null || input.Length == 0;
        }


        public static bool IsNullOrWhiteSpace (SlowString input) {
            if (input == null) {
                return true;
            }

            for (var i = 0; i < input.Length; i++) {
                if (!char.IsWhiteSpace(input[i])) {
                    return false;
                }
            }

            return true;
        }


        public SlowString[] Split (char separator) {
            return this.Split(separator, StringSplitOptions.None);
        }


        public SlowString[] Split (char separator, StringSplitOptions splitOptions) {
            if (this.chars.Length == 0) {
                return new SlowString[0];
            }

            var startIndex = 0;
            var length = 0;

            var result = new List<SlowString>();

            for (var i = 0; i < this.chars.Length; i++) {
                if (this.chars[i] == separator) {
                    if (length != 0 || splitOptions != StringSplitOptions.RemoveEmptyEntries) {
                        result.Add(new SlowString(this.chars, startIndex, length));
                    }

                    startIndex = i;
                    length = 0;
                } else {
                    length++;
                }
            }

            if (length != 0 || splitOptions != StringSplitOptions.RemoveEmptyEntries) {
                result.Add(new SlowString(this.chars, startIndex, length));
            }

            return result.ToArray();
        }


        public SlowString[] Split (SlowString separator) {
            return this.Split(separator, StringSplitOptions.None);
        }


        public SlowString[] Split (SlowString separator, StringSplitOptions splitOptions) {
            if (this.chars.Length == 0) {
                return new SlowString[0];
            }

            var startIndex = 0;
            var length = 0;

            var result = new List<SlowString>();

            var indexInSeparator = 0;
            var separatorLength = separator.Length;
            int correctedLength;

            for (var i = 0; i < this.chars.Length; i++) {
                if (this.chars[i] == separator[indexInSeparator++]) {
                    if (indexInSeparator == separatorLength) {
                        correctedLength = length - separatorLength;

                        if (correctedLength != 0 || splitOptions != StringSplitOptions.RemoveEmptyEntries) {
                            result.Add(new SlowString(this.chars, startIndex, correctedLength));
                        }

                        startIndex = i;
                        length = 0;
                    } else {
                        indexInSeparator = 0;
                    }
                } else {
                    length++;
                }
            }

            correctedLength = length - separatorLength;

            if (correctedLength != 0 || splitOptions != StringSplitOptions.RemoveEmptyEntries) {
                result.Add(new SlowString(this.chars, startIndex, correctedLength));
            }

            return result.ToArray();
        }


        public SlowString Substring (int startIndex) {
            return this.Substring(startIndex, this.Length - startIndex);
        }


        public SlowString Substring (int startIndex, int length) {
            if (length == 0) {
                return EmptySlowString;
            }

            if (startIndex == 0 && length == this.Length) {
                return this;
            }

            var arr = new char[length];

            Array.Copy(this.chars, startIndex, arr, 0, length);

            return new SlowString(arr, null);
        }


        public SlowString SubstringWithEndIndex (int startIndex, int endIndex) {
            return this.Substring(startIndex, endIndex - startIndex);
        }


        public SlowString TrimStart (params char[] trimChars) {
            return this.Trim(trimChars, true, false);
        }


        public SlowString TrimEnd (params char[] trimChars) {
            return this.Trim(trimChars, false, true);
        }


        public SlowString Trim (params char[] trimChars) {
            return this.Trim(trimChars, true, true);
        }


        private SlowString Trim (char[] trimChars, bool trimStart, bool trimEnd) {
            var startIndex = 0;
            var endIndex = this.chars.Length - 1;
            var trimCharsLength = trimChars.Length;

            if (trimStart) {
                for (; startIndex <= endIndex; startIndex++) {
                    var currentChar = this.chars[startIndex];

                    var i = 0;
                    for (; i < trimCharsLength && currentChar != trimChars[i]; i++) ;

                    if (i == trimCharsLength) {
                        break;
                    }
                }
            }

            if (trimEnd) {
                for (; endIndex >= startIndex; endIndex--) {
                    var currentChar = this.chars[endIndex];

                    var i = 0;
                    for (; i < trimCharsLength && currentChar != trimChars[i]; i++) ;

                    if (i == trimCharsLength) {
                        break;
                    }
                }
            }

            return this.SubstringWithEndIndex(startIndex, endIndex + 1);
        }


        public bool Contains (SlowString input) {
            return this.IndexOf(input) > -1;
        }


        public bool StartsWith (char input) {
            return this.chars.Length > 0 && this.chars[0] == input;
        }


        public bool StartsWith (SlowString input) {
            return this.IndexOf(input) == 0;
        }


        public bool EndsWith (char input) {
            return this.chars.Length > 0 && this.chars[this.chars.Length - 1] == input;
        }


        public bool EndsWith (SlowString input) {
            return this.LastIndexOf(input) == this.Length - input.Length;
        }


        public int IndexOf (char input) {
            return this.IndexOf(input, 0);
        }


        public int IndexOf (char input, int startIndex) {
            for (var i = startIndex; i < this.chars.Length; i++) {
                if (this.chars[i] == input) {
                    return i;
                }
            }

            return -1;
        }


        public int IndexOf (SlowString input) {
            return this.IndexOf(input, 0);
        }


        public int IndexOf (SlowString input, int startIndex) {
            if (IsNullOrEmpty(input)) {
                return startIndex;
            }

            var thisLength = this.chars.Length;
            var inputLength = input.chars.Length;

            for (var i = startIndex; i < thisLength; i++) {
                if (inputLength > thisLength - i) {
                    return -1;
                }

                var j = i;
                for (; j < inputLength; j++) {
                    if (this.chars[j] != input.chars[j]) {
                        break;
                    }
                }

                if (j == inputLength) {
                    return i;
                }
            }

            return -1;
        }


        public int IndexOfAny (char[] input) {
            return this.IndexOfAny(input, 0);
        }


        public int IndexOfAny (char[] input, int startIndex) {
            if (input == null || input.Length == 0) {
                return startIndex;
            }

            var thisLength = this.chars.Length;
            var inputLength = input.Length;

            for (var i = startIndex; i < thisLength; i++) {
                for (var j = i; j < inputLength; j++) {
                    if (this.chars[j] == input[j]) {
                        return j;
                    }
                }
            }

            return -1;
        }

        public int LastIndexOf (char input) {
            return this.LastIndexOf(input, 0);
        }


        public int LastIndexOf (char input, int startIndex) {
            for (var i = this.Length - 1; i >= startIndex; i--) {
                if (this.chars[i] == input) {
                    return i;
                }
            }

            return -1;
        }


        public int LastIndexOf (SlowString input) {
            return this.LastIndexOf(input, 0);
        }


        public int LastIndexOf (SlowString input, int startIndex) {
            var thisLength = this.chars.Length;

            if (IsNullOrEmpty(input)) {
                return thisLength - 1;
            }

            var inputLength = input.chars.Length;

            for (var i = thisLength - 1; i >= 0; i--) {
                if (inputLength > thisLength - i) {
                    return -1;
                }

                var j = inputLength;
                for (; j >= i; j--) {
                    if (this.chars[j] != input.chars[j]) {
                        break;
                    }
                }

                if (j == i) {
                    return i;
                }
            }

            return -1;
        }


        public int LastIndexOfAny (char[] input) {
            return this.LastIndexOfAny(input, 0);
        }


        public int LastIndexOfAny (char[] input, int startIndex) {
            var thisLength = this.chars.Length;

            if (IsNullOrEmpty(input)) {
                return thisLength - 1;
            }

            var inputLength = input.Length;

            for (var i = thisLength - 1; i >= startIndex; i--) {
                for (var j = 0; j < inputLength; j++) {
                    var combinedIndex = i + j;
                    if (this.chars[combinedIndex] == input[j]) {
                        return combinedIndex;
                    }
                }
            }

            return -1;
        }


        public SlowString PadLeft (int totalLength) {
            return this.PadLeft(totalLength, ' ');
        }


        public SlowString PadLeft (int totalLength, char paddingChar) {
            return this.Pad(totalLength, paddingChar, true);
        }


        public SlowString PadRight (int totalLength) {
            return this.PadRight(totalLength, ' ');
        }


        public SlowString PadRight (int totalLength, char paddingChar) {
            return this.Pad(totalLength, paddingChar, false);
        }


        private SlowString Pad (int totalLength, char paddingChar, bool padLeft) {
            var currentLength = this.Length;

            if (totalLength <= currentLength) {
                return this;
            }

            var newChars = new char[totalLength];

            var lengthsDelta = totalLength - currentLength;

            var startIndexInNewChars = padLeft ? lengthsDelta : 0;

            Array.Copy(this.chars, 0, newChars, startIndexInNewChars, currentLength);

            var startIndexForLoop = padLeft ? 0 : currentLength - 1;
            var endIndexForLoop = padLeft ? lengthsDelta : totalLength;

            for (var i = startIndexForLoop; i < endIndexForLoop; i++) {
                newChars[i] = paddingChar;
            }

            return new SlowString(newChars, null);
        }


        public SlowString Insert (int startIndex, SlowString value) {
            this.CheckIndex(startIndex);

            var currentLength = this.Length;
            var valueLength = value.Length;

            if (valueLength == 0) {
                return this;
            }

            var newChars = new char[currentLength + valueLength];

            if (startIndex > 0) {
                Array.Copy(this.chars, newChars, startIndex);
            }

            Array.Copy(value.chars, 0, newChars, startIndex, valueLength);

            Array.Copy(this.chars, startIndex, newChars, startIndex + valueLength, currentLength - startIndex - valueLength);

            return new SlowString(newChars, null);
        }


        public SlowString Replace (char first, char second) {
            var newChars = new char[this.chars.Length];

            for (var i = 0; i < this.chars.Length; i++) {
                var currentChar = this.chars[i];

                if (currentChar == first) {
                    newChars[i] = second;
                } else {
                    newChars[i] = currentChar;
                }
            }

            return new SlowString(newChars, null);
        }


        public SlowString Replace (SlowString first, SlowString second) {
            int index;
            var result = this;
            var firstLength = first.Length;

            while ((index = result.IndexOf(first)) > -1) {
                var before = result.Substring(0, index);
                var after = result.Substring(index + firstLength);

                result = before.Concat(first).Concat(after);
            }

            return result;
        }


        public SlowString Remove (int startIndex) {
            return this.Remove(startIndex, this.Length - startIndex);
        }


        public SlowString Remove (int startIndex, int count) {
            this.CheckIndex(startIndex);

            if (count == 0) {
                return this;
            }

            var currentLength = this.Length;

            if (count < 0 || currentLength - startIndex < count) {
                throw new IndexOutOfRangeException();
            }

            var newLength = currentLength - count;

            var newChars = new char[newLength];

            Array.Copy(this.chars, 0, newChars, 0, startIndex);

            var remainingLength = newLength - startIndex;

            if (remainingLength > 0) {
                Array.Copy(this.chars, startIndex + count, newChars, startIndex, remainingLength);
            }

            return new SlowString(newChars, null);
        }


        IEnumerator<char> IEnumerable<char>.GetEnumerator () {
            for (var i = 0; i < this.chars.Length; i++) {
                yield return this.chars[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return this.chars.GetEnumerator();
        }


        public static bool Equals (SlowString first, SlowString second) {
            if (ReferenceEquals(null, first)) {
                return ReferenceEquals(null, second);
            }

            return first.Equals(second);
        }


        public bool Equals (SlowString other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            var firstChars = this.chars;
            var secondChars = other.chars;

            if (firstChars.Length != secondChars.Length) {
                return false;
            }

            for (var i = 0; i < this.chars.Length; i++) {
                if (firstChars[i] != secondChars[i]) {
                    return false;
                }
            }

            return true;
        }


        public override bool Equals (object other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            var otherAsSlowString = other as SlowString;
            return otherAsSlowString != null && this.Equals(otherAsSlowString);
        }


        public override int GetHashCode () {
            unchecked {
                var result = 17;

                for (var i = 0; i < this.chars.Length; i++) {
                    result = 31 * result + this.chars[i].GetHashCode();
                }

                return result;
            }
        }


        public override string ToString () {
            return new string(this.CloneChars());
        }


        private char[] CloneChars () {
            var newChars = new char[this.chars.Length];

            Array.Copy(this.chars, newChars, this.chars.Length);

            return newChars;
        }


        public char[] ToCharArray () {
            return this.CloneChars();
        }

    }

}
