# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
