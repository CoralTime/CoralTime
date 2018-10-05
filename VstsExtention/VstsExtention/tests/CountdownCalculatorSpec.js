"use strict";
// ---------------------------------------------------------------------
// <copyright file="CountdownCalculatorSpec.ts">
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>
//    This is part of the Countdown widget
//    from the ALM Rangers. This file contains the unit tests
//    for the countdown calculator.
// </summary>
// ---------------------------------------------------------------------
exports.__esModule = true;
var moment = require("moment-timezone");
var CountdownCalculator = require("../src/countdownCalculator");
var CountdownResult = require("../src/CountdownResult");
describe("countdown ", function () {
    it("from date before to date is valid", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("21122015", "DDMMYYYY"), moment("01012016", "DDMMYYYY"), false);
        expect(calculator.isValid()).toBe(true);
    });
    it("from date after to date is invalid", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("01012016", "DDMMYYYY"), moment("21122015", "DDMMYYYY"), false);
        expect(calculator.isValid()).toBe(false);
    });
    it("from 1-12-2015 to 31-12-2015 is 30 days", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("1-12-2015", "DD-MM-YYYY"), moment("31-12-2015", "DD-MM-YYYY"), false);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBe(30);
        expect(countdownResult.unit).toBe(CountdownResult.Unit.Days);
    });
    it("from 1-12-2015 to 2-12-2015 is 1 day", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("01-12-2015", "DD-MM-YYYY"), moment("02-12-2015", "DD-MM-YYYY"), false);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBe(1);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Days");
    });
    it("from 1-12-2015 10:00 to 1-12-2015 11:00 is 1 hour", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("01-12-2015 10:00", "DD-MM-YYYY HH"), moment("01-12-2015 11:00", "DD-MM-YYYY HH"), false);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBe(1);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Hours");
    });
    it("from 1-12-2015 10:00 to 1-12-2015 10:59 is 59 minutes", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("01-12-2015 10:00", "DD-MM-YYYY HH:mm"), moment("01-12-2015 10:59", "DD-MM-YYYY HH:mm"), false);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBe(59);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Minutes");
    });
    it("from 1-12-2015 10:00:00 to 1-12-2015 10:00:59 is 59 seconds", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("01-12-2015 10:00:00", "DD-MM-YYYY HH:mm:ss"), moment("01-12-2015 10:00:59", "DD-MM-YYYY HH:mm:ss"), false);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBe(59);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Seconds");
    });
    it("invalid countdown to return a difference of 0", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment("02-12-2015", "DD-MM-YYYY"), moment("01-12-2015", "DD-MM-YYYY"), false);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBe(0);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Invalid");
    });
    it("countdown from 01-12-2015 10:00 Europe/Amsterdam to 01-12-2015 10:00 America/Los_Angeles to be 9 hours", function () {
        var calculator = new CountdownCalculator.CountdownCalculator(moment.tz("01-12-2015 10:00", "DD-MM-YYYY H:m", "Europe/Amsterdam"), moment.tz("01-12-2015 10:00", "DD-MM-YYYY H:m", "America/Los_Angeles"), false);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBe(9);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Hours");
    });
    it("use case 1 : countdown from 06-10-2016 14:20 Europe/Paris to 11-10-2016 23:59 Europe/Paris to be 3 days", function () {
        var DayOfWeeks = ["monday", "tuesday", "wednesday", "thursday", "friday"];
        var calculator = new CountdownCalculator.CountdownCalculator(moment.tz("06-10-2016 14:00", "DD-MM-YYYY H:m", "Europe/Paris"), moment.tz("11-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris"), false, DayOfWeeks);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBeGreaterThan(3.4);
        expect(countdownResult.value).toBeLessThan(3.5);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Days");
    });
    it("use case 2: (other non working day) countdown from 06-10-2016 14:20 Europe/Paris\n\t\t\tto 11-10-2016 23:59 Europe/Paris to be 3 days", function () {
        var DayOfWeeks = ["sunday", "wednesday", "thursday", "friday", "saturday"];
        var calculator = new CountdownCalculator.CountdownCalculator(moment.tz("06-10-2016 14:00", "DD-MM-YYYY H:m", "Europe/Paris"), moment.tz("11-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris"), false, DayOfWeeks);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value).toBeGreaterThan(3.4);
        expect(countdownResult.value).toBeLessThan(3.5);
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Days");
    });
    it("use case 3 : diff from 16-10-2016 11:38 Europe/Paris to 30-10-2016 23:59 Europe/Paris\n\t\t\tWith skip no-working days to be 14.5 days", function () {
        var DayOfWeeks = ["monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"];
        var calculatorWithSkip = new CountdownCalculator.CountdownCalculator(moment.tz("16-10-2016 11:38", "DD-MM-YYYY H:m", "Europe/Paris"), moment.tz("30-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris"), false, DayOfWeeks);
        var countdownResultWithSkip = calculatorWithSkip.getDifference();
        expect(countdownResultWithSkip.value.toFixed(1)).toBe("14.5");
    });
    it("use case 3 bis: diff from 13-10-2016 11:38 Europe/Paris to 30-10-2016 23:59 Europe/Paris\n\t\t\tWith skip no-working days to be 19 days", function () {
        var DayOfWeeks = ["monday", "tuesday", "wednesday", "thursday", "friday"];
        var calculatorWithSkip = new CountdownCalculator.CountdownCalculator(moment.tz("13-10-2016 11:38", "DD-MM-YYYY H:m", "Europe/Paris"), moment.tz("30-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris"), false, DayOfWeeks);
        var countdownResultWithSkip = calculatorWithSkip.getDifference();
        expect(countdownResultWithSkip.value.toFixed(1)).toBe("11.5");
    });
    it("use case 4 : diff from 16-10-2016 11:38 Europe/Paris to 30-10-2016 23:59 Europe/Paris\n\t\t\tWith No skip no-working days to be 19 days", function () {
        var DayOfWeeks = ["monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"];
        var calculatorNoSkip = new CountdownCalculator.CountdownCalculator(moment.tz("16-10-2016 11:38", "DD-MM-YYYY H:m", "Europe/Paris"), moment.tz("30-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris"), false);
        var countdownResultNoSkip = calculatorNoSkip.getDifference();
        expect(countdownResultNoSkip.value.toFixed(1)).toBe("14.5");
    });
    it("use case 5: diff from 23-10-2016 11:38 Europe/Paris to 30-10-2016 23:59 Europe/Paris\n\t\t\tWith skip no-working days to be 7 days", function () {
        var DayOfWeeks = [0, 1, 2, 3, 4, 5, 6];
        var calculator = new CountdownCalculator.CountdownCalculator(moment.tz("23-10-2016 11:38", "DD-MM-YYYY H:m", "Europe/Paris"), moment.tz("30-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris"), false, DayOfWeeks);
        var countdownResult = calculator.getDifference();
        expect(countdownResult.value.toFixed(1)).toBe("7.5");
        expect(CountdownResult.Unit[countdownResult.unit]).toBe("Days");
    });
});
describe("diff method ", function () {
    it("diff from 06-10-2016 14:20 Europe/Paris to 11-10-2016 02:00 Europe/Paris to be 5 days", function () {
        var from = moment.tz("06-10-2016 14:00", "DD-MM-YYYY H:m", "Europe/Paris");
        var to = moment.tz("11-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris");
        var datediff = to.diff(from, "days", false);
        expect(datediff).toBe(5);
    });
    it("round diff from 06-10-2016 14:20 Europe/Paris to 11-10-2016 02:00 Europe/Paris to be 5 days", function () {
        var from = moment.tz("06-10-2016 14:00", "DD-MM-YYYY H:m", "Europe/Paris");
        var to = moment.tz("11-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris");
        var datediff = to.diff(from, "days", true);
        expect(datediff).toBeGreaterThan(5);
        expect(datediff).toBeLessThan(6);
    });
    it("diff from 16-10-2016 11:38 Europe/Paris to 30-10-2016 23:59 Europe/Paris\n\t\t\tWith no skip no-working days to be 19 days", function () {
        var from = moment.tz("16-10-2016 11:38", "DD-MM-YYYY H:m", "Europe/Paris");
        var to = moment.tz("30-10-2016 23:59", "DD-MM-YYYY H:m", "Europe/Paris");
        var datediff = to.diff(from, "days", false);
        expect(datediff).toBe(14);
    });
});
