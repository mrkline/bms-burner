using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bms_burner
{
    public struct BMSData
    {

        float x;            // Ownship North (Ft)
        float y;            // Ownship East (Ft)
        float z;            // Ownship Down (Ft) --- NOTE: use FlightData2 AAUZ for barometric altitude!
        float xDot;         // Ownship North Rate (ft/sec)
        float yDot;         // Ownship East Rate (ft/sec)
        float zDot;         // Ownship Down Rate (ft/sec)
        float alpha;        // Ownship AOA (Degrees)
        float beta;         // Ownship Beta (Degrees)
        float gamma;        // Ownship Gamma (Radians)
        float pitch;        // Ownship Pitch (Radians)
        float roll;         // Ownship Pitch (Radians)
        float yaw;          // Ownship Pitch (Radians)
        float mach;         // Ownship Mach number
        float kias;         // Ownship Indicated Airspeed (Knots)
        float vt;           // Ownship True Airspeed (Ft/Sec)
        float gs;           // Ownship Normal Gs
        float windOffset;   // Wind delta to FPM (Radians)
        float nozzlePos;    // Ownship engine nozzle percent open (0-100)
                            //float nozzlePos2;   // MOVED TO FlightData2! Ownship engine nozzle2 percent open (0-100) 
        float internalFuel; // Ownship internal fuel (Lbs)
        float externalFuel; // Ownship external fuel (Lbs)
        float fuelFlow;     // Ownship fuel flow (Lbs/Hour)
        float rpm;          // Ownship engine rpm (Percent 0-103)
                            //float rpm2;         // MOVED TO FlightData2! Ownship engine rpm2 (Percent 0-103)
        float ftit;         // Ownship Forward Turbine Inlet Temp (Degrees C)
                            //float ftit2;        // MOVED TO FlightData2! Ownship Forward Turbine Inlet Temp2 (Degrees C)
        float gearPos;      // Ownship Gear position 0 = up, 1 = down;
        float speedBrake;   // Ownship speed brake position 0 = closed, 1 = 60 Degrees open
        float epuFuel;      // Ownship EPU fuel (Percent 0-100)
        float oilPressure;  // Ownship Oil Pressure (Percent 0-100)
                            //float oilPressure2; // MOVED TO FlightData2! Ownship Oil Pressure2 (Percent 0-100)
        uint lightBits;    // Cockpit Indicator Lights, one bit per bulb. See enum

        // These are inputs. Use them carefully
        // NB: these do not work when TrackIR device is enabled
        // NB2: launch falcon with the '-head' command line parameter to activate !
        float headPitch;    // Head pitch offset from design eye (radians)
        float headRoll;     // Head roll offset from design eye (radians)
        float headYaw;      // Head yaw offset from design eye (radians)

        // new lights
        uint lightBits2;   // Cockpit Indicator Lights, one bit per bulb. See enum
        uint lightBits3;   // Cockpit Indicator Lights, one bit per bulb. See enum

        // chaff/flare
        float ChaffCount;   // Number of Chaff left
        float FlareCount;   // Number of Flare left

        // landing gear
        float NoseGearPos;  // Position of the nose landinggear; caution: full down values defined in dat files
        float LeftGearPos;  // Position of the left landinggear; caution: full down values defined in dat files
        float RightGearPos; // Position of the right landinggear; caution: full down values defined in dat files

        // ADI values
        float AdiIlsHorPos; // Position of horizontal ILS bar
        float AdiIlsVerPos; // Position of vertical ILS bar

        // HSI states
        int courseState;    // HSI_STA_CRS_STATE
        int headingState;   // HSI_STA_HDG_STATE
        int totalStates;    // HSI_STA_TOTAL_STATES; never set

        // HSI values
        float courseDeviation;     // HSI_VAL_CRS_DEVIATION
        float desiredCourse;       // HSI_VAL_DESIRED_CRS
        float distanceToBeacon;    // HSI_VAL_DISTANCE_TO_BEACON
        float bearingToBeacon;     // HSI_VAL_BEARING_TO_BEACON
        float currentHeading;      // HSI_VAL_CURRENT_HEADING
        float desiredHeading;      // HSI_VAL_DESIRED_HEADING
        float deviationLimit;      // HSI_VAL_DEV_LIMIT
        float halfDeviationLimit;  // HSI_VAL_HALF_DEV_LIMIT
        float localizerCourse;     // HSI_VAL_LOCALIZER_CRS
        float airbaseX;            // HSI_VAL_AIRBASE_X
        float airbaseY;            // HSI_VAL_AIRBASE_Y
        float totalValues;         // HSI_VAL_TOTAL_VALUES; never set

        float TrimPitch;  // Value of trim in pitch axis, -0.5 to +0.5
        float TrimRoll;   // Value of trim in roll axis, -0.5 to +0.5
        float TrimYaw;    // Value of trim in yaw axis, -0.5 to +0.5

        // HSI flags
        public uint hsiBits;      // HSI flags
    }
}
