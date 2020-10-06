(function (global, factory) {
  typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
  typeof define === 'function' && define.amd ? define(factory) :
  (global['Snowflake-ID'] = factory());
}(this, (function () { 'use strict';

  /**
   * A function for converting hex <-> dec w/o loss of precision.
   * By Dan Vanderkam http://www.danvk.org/hex2dec.html
   */

  // Adds two arrays for the given base (10 or 16), returning the result.
  // This turns out to be the only "primitive" operation we need.
  function add(x, y, base) {
    var z = [];
    var n = Math.max(x.length, y.length);
    var carry = 0;
    var i = 0;
    while (i < n || carry) {
      var xi = i < x.length ? x[i] : 0;
      var yi = i < y.length ? y[i] : 0;
      var zi = carry + xi + yi;
      z.push(zi % base);
      carry = Math.floor(zi / base);
      i++;
    }
    return z;
  }

  // Returns a*x, where x is an array of decimal digits and a is an ordinary
  // JavaScript number. base is the number base of the array x.
  function multiplyByNumber(num, x, base) {
    if (num < 0) return null;
    if (num == 0) return [];

    var result = [];
    var power = x;
    while (true) {
      if (num & 1) {
        result = add(result, power, base);
      }
      num = num >> 1;
      if (num === 0) break;
      power = add(power, power, base);
    }

    return result;
  }

  function parseToDigitsArray(str, base) {
    var digits = str.split("");
    var ary = [];
    for (var i = digits.length - 1; i >= 0; i--) {
      var n = parseInt(digits[i], base);
      if (isNaN(n)) return null;
      ary.push(n);
    }
    return ary;
  }

  function convertBase(str, fromBase, toBase) {
    var digits = parseToDigitsArray(str, fromBase);
    if (digits === null) return null;

    var outArray = [];
    var power = [1];
    for (var i = 0; i < digits.length; i++) {
      // invariant: at this point, fromBase^i = power
      if (digits[i]) {
        outArray = add(outArray, multiplyByNumber(digits[i], power, toBase), toBase);
      }
      power = multiplyByNumber(fromBase, power, toBase);
    }

    var out = "";
    for (var i = outArray.length - 1; i >= 0; i--) {
      out += outArray[i].toString(toBase);
    }
    return out;
  }

  function hexToDec(hexStr) {
    if (hexStr.substring(0, 2) === "0x") hexStr = hexStr.substring(2);
    hexStr = hexStr.toLowerCase();
    return convertBase(hexStr, 16, 10);
  }

  var classCallCheck = function (instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  };

  var createClass = function () {
    function defineProperties(target, props) {
      for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) descriptor.writable = true;
        Object.defineProperty(target, descriptor.key, descriptor);
      }
    }

    return function (Constructor, protoProps, staticProps) {
      if (protoProps) defineProperties(Constructor.prototype, protoProps);
      if (staticProps) defineProperties(Constructor, staticProps);
      return Constructor;
    };
  }();

  var Snowflake = function () {
    function Snowflake(options) {
      classCallCheck(this, Snowflake);

      options = options || {};
      this.seq = 0;
      this.mid = (options.mid || 1) % 1023;
      this.offset = options.offset || 0;
      this.lastTime = 0;
    }

    createClass(Snowflake, [{
      key: "generate",
      value: function generate() {
        var time = Date.now(),
            bTime = (time - this.offset).toString(2);

        // get the sequence number
        if (this.lastTime == time) {
          this.seq++;

          if (this.seq > 4095) {
            this.seq = 0;
          }
        } else {
          this.seq = 0;
        }

        this.lastTime = time;

        var bSeq = this.seq.toString(2),
            bMid = this.mid.toString(2);

        // create sequence binary bit
        while (bSeq.length < 12) {
          bSeq = "0" + bSeq;
        }while (bMid.length < 10) {
          bMid = "0" + bMid;
        }var bid = bTime + bMid + bSeq;
        var id = "";

        for (var i = bid.length; i > 0; i -= 4) {
          id = parseInt(bid.substring(i - 4, i), 2).toString(16) + id;
        }

        return hexToDec(id);
      }
    }]);
    return Snowflake;
  }();

  return Snowflake;

})));
