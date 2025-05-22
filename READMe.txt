# Business Process Rule Plugin for Dataverse

## Description
This plugin enforces a business rule on a Dataverse table to prevent changing the status from **Completed** (`Zakończony`) back to **Planned** (`Planowany`).  
Its purpose is to ensure data integrity by blocking invalid status transitions. You can change the table / column if you need. Use only logical names of columns/tables.

## Features
- Blocks status changes from **Completed** to **Planned** on the specified Dataverse table.
- Runs automatically during the status update operation.

## Supported Environments
- Microsoft Dataverse
- Dynamics 365

## Installation
1. Build the plugin assembly using Visual Studio.
2. Register the plugin assembly in the Dataverse Plugin Registration tool.
3. Register the plugin step on the Update message for the target entity and attribute (status/statuscode).
4. Register the plugin PreImage feature.
5. Configure the step to trigger in the appropriate pipeline stage (e.g., Pre-operation).

## Usage
- After registration, the plugin will automatically prevent users or processes from changing the status from **Completed** to **Planned**.
- Attempts to do so will result in an error, blocking the update.

## Configuration
- Ensure the plugin is registered on the correct entity.
- Confirm the plugin triggers on status or statuscode attribute changes.

## .NET
4.7.1

##Signder without password

## Contact
For questions or support, contact: [michal.karpinsky@gmail.com]

