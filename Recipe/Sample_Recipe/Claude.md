# Project Context & AI Guidelines (GxP & 21 CFR Part 11 Compliant)

## 1. Project Overview
- **Platform:** Beijer iX Developer (PC Target / iX PC Runtime).
- **Tech Stack:** C# (Code-behind), .NET Framework, WPF (for UI rendering), SQLite (Built-in DB).
- **Core Task:** HMI/SCADA application for a pharmaceutical cartoning machine. Integrates Cognex DataMan SDK (DM262) and a strict GxP Audit Trail system.

## 2. Regulatory Compliance Reregulations (Strict)
- **Standards:** FDA 21 CFR Part 11, EU GMP Annex 11, PIC/S Annex 11 (Data Integrity - ALCOA+).
- **No Deletion/Modification:** Audit Trail records must be append-only. The application must NEVER include any code that executes `DELETE` or `UPDATE` queries on the Audit Trail database.
- **Attributable:** Every critical action or parameter change must be linked to a specific, authenticated User ID with a contemporaneous timestamp.
- **Reason for Change:** Any modification to critical process parameters (e.g., machine speed, recipe setpoints) MUST force the operator to input a reason before the change is committed.

## 3. Strict Coding Rules

### A. UI and Threading (The Beijer iX Rule)
- UI is WPF-based. Hardware events (Cognex `ImageArrived`) run on background threads.
- **ALWAYS** use `Globals.Tags.Dispatcher.Invoke(new Action(() => { ... }));` to update UI elements from background threads to avoid cross-thread crashes.

### B. Cognex SDK Integration
- Do NOT use Cognex WinForms UI controls in the iX WPF designer.
- Handle all camera connections, triggers, and image processing purely in C# Code-behind.
- Stream live view by saving images to a temporary local path (e.g., `C:\Temp\live.jpg`) and refreshing the iX `Picture` object's `.ImageName` property within the UI thread.

### C. GxP Audit Trail & Parameter Interception
- When modifying critical setpoints, intercept the action using a temporary C# variable or pop-up screen to capture the "Reason for Change".
- Combine the tag value change with the captured reason text and the logged-in user (`Globals.Tags.SystemTagCurrentUser.Value`) before finalizing the record into the Audit Trail database.

## 4. Communication Style
- Provide clean, robust, production-ready C# code with defensive programming (try-catch, null checks).
- Ensure high memory management discipline (always unsubscribe events ` -= ` on screen closing to prevent memory leaks in 24/7 systems).