# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2021-12-22
### Fixed
- Some bugs

## [0.5.0] - 2021-12-17
### Added
- Query endpoint "Appointments" to view and filter available appointments.
- Query endpoint "EducationalContent" to view and filter available educational content.
- Query endpoint "InternalNews" to view and filter available internal news.
- Query endpoint "Mail" to view and filter available mails.
- Mutation endpoint "CreateAppointment" to add an appointment to the application.
- Mutation endpoint "UpdateAppointment" to update an appointment to the application.
- Mutation endpoint "RemoveAppointment" to remove an appointment from the application.
- Mutation endpoint "CreateEducationalContent" to add an educational content to the application.
- Mutation endpoint "UpdateEducationalContent" to update an educational content to the application.
- Mutation endpoint "PublishEducationalContent" to publish an educational content and send a push notification.
- Mutation endpoint "HideEducationalContent" to remove the published status from an educational content.
- Mutation endpoint "RemoveEducationalContent" to remove an educational content from the application.
- Mutation endpoint "CreateInternalNews" to add an internal news to the application.
- Mutation endpoint "UpdateInternalNews" to update an internal news to the application.
- Mutation endpoint "PublishInternalNews" to publish an internal news and send a push notification.
- Mutation endpoint "HideInternalNews" to remove the published status from an internal news.
- Mutation endpoint "RemoveInternalNews" to remove an internal news from the application.
- Mutation endpoint "SendMail" to send inquiry mail from the application.

### Fixed
- Department manager can update own content bug

## [0.4.2] - 2021-11-30
### Changed
- User can have multiple departments

## [0.4.1] - 2021-11-08
### Added
- Query endpoint "ExternalNews" to view and filter available external news.
- Mutation endpoint "CreateExternalNews" to add an external news to the application.
- Mutation endpoint "UpdateExternalNews" to update an external news to the application.
- Mutation endpoint "PublishExternalNews" to publish an external news and send a push notification.
- Mutation endpoint "HideExternalNews" to remove the published status from an external news.
- Mutation endpoint "RemoveExternalNews" to remove an external news from the application.

## [0.4.0] - 2021-11-03
### Changed
- Updated dependencies and removed deprecated commands

## [0.3.0] - 2021-06-08
### Added
- Query endpoint "Users" to view and filter available users.
- Mutation endpoint "CreateUser" to add a user to the application.
- Mutation endpoint "UpdateUser" to update a user of the application.
- Mutation endpoint "RemoveUser" to remove a user from the application.
- Mutation endpoint "CreateDeploymentPlan" to replace "AddDeploymentPlan".
- Mutation endpoint "CreateDepartment" to add a Department to the application.
- Mutation endpoint "UpdateDepartment" to update a Department of the application.
- Mutation endpoint "RemoveDepartment" to remove a Department from the application.

## Changed
- Permissions are no longer global but can be defined more specific in the users access control list.
- Adjusted mutation endpoint "PublishDeploymentPlan" to only send notifications to the department linked to the deployment plan.
- Mutation endpoint "UpdateDeploymentPlan" to include the department.
- Mutation endpoint "RemoveDeploymentPlan" to include the department.

### Removed
- Mutation endpoint "AddDeploymentPlan" for consistent naming.
- Mutation endpoint "SendPushTo" since it is no longer necessary due to the "PublishDeploymentPlan" endpoint.

## [0.2.0] - 2021-05-18
### Added
- Mutation endpoint "RegisterFcmToken" to register a fcm device token.
- Mutation endpoint "UnregisterFcmToken" to remove a fcm device token.
- Mutation endpoint "UpdateDeploymentPlan" to adjust a deployment plan.
- Mutation endpoint "RemoveDeploymentPlan" to remove a deployment plan.
- Mutation endpoint "PublishDeploymentPlan" to publish a deployment plan and send a push notification.
- Mutation endpoint "HideDeploymentPlan" to remove the published status from a deplyoment plan.
- Some unit tests

### Changed
- Improved logging.
- Improved error handling.
- Errors now return a traceId in order to correlate the error with the server.

## [0.1.0] - 2021-04-06
### Added
- Query endpoint "Authenticate" which allows a user to get an authentication token.
- Query endpoint "DeploymentPlans" to view and filter available deployment plans.
- Mutation endpoint "AddDeploymentPlan" to add a deployment plan.
- Mutation endpoint "SendPushTo" to send a push notification to a device.
- Authentication with username and password.

## [0.0.1] - 2021-03-24
### Changed
- Created CI
