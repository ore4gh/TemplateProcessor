# Quick Start Guide - TextList Generator

## What Was Done

✅ Created new Windows Form: **FormTextListGenerator**
✅ Added to **Sequence Editor** menu in MainForm
✅ Integrated with existing TemplateProcessor project
✅ Uses ClosedXML (already in your project)
✅ Uses existing SqlTools class

## Location

```
TemplateProcessor/
├── TemplateProcessor/
│   ├── Sequence/
│   │   ├── FormTextListGenerator.cs          ← NEW
│   │   ├── FormTextListGenerator.Designer.cs ← NEW
│   │   ├── FormTextListGenerator.resx        ← NEW
│   │   └── IMPLEMENTATION_SUMMARY.md         ← NEW (detailed docs)
│   ├── MainForm.cs                            ← MODIFIED (menu enabled)
│   ├── TemplateProcessor.csproj               ← MODIFIED (Template.xml copy)
│   └── Template.xml                           ← EXISTING (verified format)
```

## How to Use

### Step 1: Build Solution
```
Press F6 or click Build → Build Solution
```

### Step 2: Run Application
```
Press F5 or click Debug → Start Debugging
```

### Step 3: Open Form
```
Click menu: "Sequence editor"
```

### Step 4: Generate TextList
```
1. Select sheet from ComboBox (shows TextEq_Number sheets)
2. Click "Generate Text" button
3. Check log panel for progress
4. Verify output file created in project folder
5. Verify database updated
```

## Form UI Layout

```
┌────────────────────────────────────────────────────────┐
│  TextList Generator                                    │
│                                                         │
│  ┌─ Sheet Selection ─────────────────────────────────┐ │
│  │                                                    │ │
│  │  Select Sheet (TextEq_Number):                    │ │
│  │  ┌────────────────────────┐  ┌──────────────┐    │ │
│  │  │ TextEq_001            ▼│  │Reload Sheets │    │ │
│  │  └────────────────────────┘  └──────────────┘    │ │
│  │                                                    │ │
│  └────────────────────────────────────────────────────┘ │
│                                                         │
│  ┌─ Actions ────────────────────────────────────────┐  │
│  │                                                   │  │
│  │  ┌─────────────────┐                             │  │
│  │  │  Generate Text  │                             │  │
│  │  └─────────────────┘                             │  │
│  │                                                   │  │
│  └───────────────────────────────────────────────────┘  │
│                                                         │
│  Instructions:                                         │
│  1. Select a sheet from the dropdown                   │
│  2. Click 'Generate Text' to process                   │
│  3. Output file created in project folder              │
│  4. Database TextLists table updated                   │
│                                                         │
└────────────────────────────────────────────────────────┘
```

## What Happens When You Click "Generate Text"

1. **Reads Excel** (Tmp_[filename].xlsm)
   - Skips first 3 rows
   - Processes up to 1854 rows
   - Filters by columns 7, 8, 9

2. **Processes Templates**
   - tEqStep (column 7) - Step names
   - tEqTrans (column 8) - Transition names
   - tEqCond (column 9) - Condition names

3. **Generates Output**
   - Creates text file: [SheetName].txt
   - Saves to project folder

4. **Updates Database**
   - Deletes old entries for TagName_Eq
   - Inserts new entries
   - Uses stored procedure SP_TextList_Write

5. **Shows Result**
   - Success message with counts
   - All actions logged to bottom panel

## Required Configuration

Make sure these are set in your TemplateProcessorSettings.json:

```json
{
  "CommonData": {
    "SharedFolderPath": "Z:\\Senja_GenData",
    "ProjectFolderName": "YourProject",
    "ExcelDataFileName": "Components_....xlsm",
    "SqlServerName": "CKB30ES002\\WINCC",
    "SqlDatabaseName": "Senja_Production",
    "SqlUsername": "sa",
    "SqlPassword": "YourPassword"
  }
}
```

## Before First Use

✓ Ensure Template.xml is in project folder (it is! ✓)
✓ Click "Reload Excel" menu first to create Tmp_ file
✓ Verify Excel has sheets starting with "TextEq_"
✓ Check SQL Server connection is working

## Troubleshooting

| Error | Solution |
|-------|----------|
| Template.xml not found | Build solution to copy file to output |
| Excel file not found | Click "Reload Excel" menu item |
| No sheets found | Check Excel has TextEq_* sheets |
| SQL error | Verify connection settings in config |

## Example Workflow

```
1. Start application
2. (If needed) Click "Reload Excel" menu
3. Click "Sequence editor" menu
4. Select "TextEq_001" from dropdown
5. Click "Generate Text"
6. Wait for success message
7. Check log for details:
   - "Excel sheet 'TextEq_001' read successfully"
   - "Processed 45 step entries"
   - "Processed 12 transition entries"
   - "Processed 8 condition entries"
   - "Inserted/updated 65 TextList entries"
   - "TextList generation completed successfully"
```

## Output Files

**Text File:** `[SheetName].txt`
```
Location: Z:\Senja_GenData\YourProject\
Format: Tab-separated values
Encoding: UTF-8
Content: Processed template data sorted by ID
```

**Database:** `dbo.TextLists`
```
TextList: TagName_Eq
Records: All processed entries
Format: L1_Text, L2_Text, L3_Text, L4_Text, L5_Text
```

## Log Messages You'll See

```
=== Starting TextList generation for sheet: TextEq_001 ===
Excel sheet 'TextEq_001' read successfully. Rows: 1854
TagName from column 2: YourTagName
Processed 45 step entries
Processed 12 transition entries
Processed 8 condition entries
Total entries after sorting: 65
Output written to: Z:\Senja_GenData\YourProject\TextEq_001.txt
Deleting existing TextList entries for: YourTagName_Eq
Deleted 60 rows from dbo.TextLists where TextList = YourTagName_Eq
Inserted/updated 65 TextList entries
=== TextList generation completed successfully ===
```

## Success!

Your Python script is now fully integrated as a Windows Forms application in your TemplateProcessor project! 🎉

The form is accessible from the **"Sequence editor"** menu and provides a user-friendly interface for generating TextLists from Excel data.

For detailed technical information, see: `IMPLEMENTATION_SUMMARY.md`
