namespace Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;

    public static class TestDataProvider
    {
        public static void InsertTestData(ProntoMiaDbContext context)
        {
            InsertUsers(context);
            InsertTokens(context);
            InsertDepartments(context);
        }

        [SuppressMessage(
            "Menees.Analyzers",
            "MEN002",
            Justification = "Inline hashes and salts.")]
        private static void InsertUsers(ProntoMiaDbContext context)
        {
            if (!context.Users.Any())
            {
                // Password: Hello Alice
                var bob = new User(
                    "Bob",
                    Convert.FromBase64String("WYEhHn7odNFZ1DIDi/QU9xGsyrJn7bjdbC20DYWsWPtE3Csj7tUwEkwN/YdTk0Mh9MytigynkRgb6UD660zvh15Dop9bm9dyjAwimUGPuOQVzPbsOWJdZPUo1tBadcWPlbhY4fOsYCKiYYVowHZJN4RbUddomOCePONDeR3vUpR5ClQiAPLr/FqWCcmbiJFNW3Dfq/gsh2YUQxRWN7yyD8/E9Op/XC3qZXmkSrRCpk8aujPaDSoRcbBaMoySpnXBfagTPAOoUgv2RtCawXjLZsytprwvhmb59jyy8a7hrsklVmqqv07bbRtonWIVxz5as7jOeOuGDcmKE6sOIh3kNuYWNFkfgYFr0uP5bClYbBNZRWFRB43RmQNyr/pqXoAGGdoc9/dKDdYb1vExxrUqBtOr2rDQWzPGFq2J/fBDuSjiVNNG/jYkGhXr3SsXXkEvmThGafjZWgVlkE7dPlQ0yFVxdtdznDtjhry9jL2FHWZRvKVI0oWw3wG6Gkb2nC7ralOiPbDjL3y+44odaX6oHl8lW3CERpMu4qW3vr/UluT4M7lrekRd1ljoqG//MwBauDKuHoxKZ6NYiR0nf9EURJvuzJy+8sJJfgMLsWDZSnk0qIDrlSrImUHjKqK1VjzKW4OrgVfCcCL1c89L0rR/P8CfM5jRVIgI78jIRWcX520="),
                    "Pbkdf2Generator",
                    "{\"SaltSize\":128,\"HashIterations\":500,\"HashSize\":512,\"Salt\":\"tGnRdutheZwbKKAz0I0YqsUPiWU4AVAndLvLfH5hYLFnVR0cn9FQzGj+Z88gcfopw0SWA8IjzLSksSADt+SZILaIM57pPC2KoPMfuS6iSXv9i/+pKxBN5EaZS36s/k4lHbTqc9sy2vMXhr0J5GvL68oJbgMZchXx6zaYSkoABX0=\"}");

                // Password: Hello Bob
                var alice = new User(
                    "Alice",
                    Convert.FromBase64String("VcNnsN+lbbYR478VMG7OM1Yfp4dAFfsSQ/JSNhrr/2+rygSWGNXPNG1B24q7KQ1YaVnkZGSsJIDhMXZ6r4nwsmXMObNcPWrFcVlnCoMOUYXJvSOMSwqZ6q7V225JK3MxQp8InvV2dTISfWq3jRuqQPog5mSd/HwIUmOxY2IwdqQXpDed9fAtzwQDRQvtxOf/T9hWi37MZXVPSXmY6Zsis4JFwtEv/qF6SZu+lPt6jTPfjJ2U595nOufRG0axPRqRrr3mVqpv0TNPUey4+GX7pgCsrX6zg9eExiGLWPcPHYf8jW8SEWg3dRqRsIyYFRqdHUOze+CzXe0BhVpbcFp788Jv7hM3y28dpDPh5QySexXSCQEt6Ihms7aPQQW+Eyot8t+P73NLOuiLrwzCcXPcxo4lt6DA3OLjCYmSW4sifGTkpKQFg+0Srw8o4AzsDUgLwjShJfx9bNTJjzOSprtoAL4dLa/yAmcCyisuxueejQJxec3u1g+w5R3ms94ewKjChR+8eEZF/0EdaU0zyYMMqTeJxW6Bwy6atZxrvJadwCAvjbKvhAd5qTTMSaNCiyVOV70Y0zxTU+0biIpI9gwOZB0ljUXUX+vHhazYzZJ3QlJDJt2x99rCSxkT4MqDI6yUmLQzMQdv8KxEImo/yjgMum9TzLAZJpdrIdCe/i4qrUE="),
                    "Pbkdf2Generator",
                    "{\"SaltSize\":128,\"HashIterations\":500,\"HashSize\":512,\"Salt\":\"qXcYhSEgpE6lBpofTVk2SosdJUswxiXl+5asVtKFlH18iUcdmc/1lGqIyCDwp1ZBX1WpEOXhI6QzWMIcayuKotU+1it79ojyuTiIONOpHkzzv63BYeS5D8M5KxAzQoTJ1QMmvNVWf9tIJ/YBpQ2oCwPn0Zx72A4d2fyKbOhdtEQ=\"}");

                context.Users.AddRange(bob, alice);
                context.SaveChanges();
            }
        }

        private static void InsertTokens(ProntoMiaDbContext context)
        {
            if (!context.DeploymentPlans.Any())
            {
                var deploymentPlan1 = new DeploymentPlan(
                    DateTime.Parse("2018-08-12"),
                    DateTime.Parse("2018-09-01"),
                    Guid.NewGuid(),
                    "exe");
                var deploymentPlan2 = new DeploymentPlan(
                    DateTime.Parse("2019-08-12"),
                    DateTime.Parse("2019-09-01"),
                    Guid.NewGuid(),
                    "pdf");

                context.DeploymentPlans.AddRange(
                    deploymentPlan1, deploymentPlan2);
                context.SaveChanges();
            }
        }

        private static void InsertDepartments(ProntoMiaDbContext context)
        {
            if (!context.Departments.Any())
            {
                var department1 = new Department(
                    "Administration");
                var department2 = new Department(
                    "Finance");

                context.Departments.AddRange(
                    department1, department2);
                context.SaveChanges();
            }
        }
        
        private static void InsertDeploymentPlans(ProntoMiaDbContext context)
        {
            if (!context.DeploymentPlans.Any())
            {
                var deploymentPlan1 = new DeploymentPlan(
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    Guid.NewGuid(), 
                    ".exe",
                    "First test plan");
                var deploymentPlan2 = new DeploymentPlan(
                    DateTime.MinValue.AddDays(2),
                    DateTime.MaxValue.AddDays(-2),
                    Guid.NewGuid(),
                    ".exe",
                    "First test plan");

                context.DeploymentPlans.AddRange(
                    deploymentPlan1, deploymentPlan2);
                context.SaveChanges();
            }
        }
    }
}