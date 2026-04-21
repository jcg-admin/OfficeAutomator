# Namespacing Guide - OfficeAutomator.Core

## Namespace Convention

The namespace convention reflects the semantic folder structure:

```
OfficeAutomator.Core.<SemanticFolder>
```

This means:
- Physical location in filesystem determines namespace
- Namespace name accurately describes class responsibility
- Easy to understand where a class is located from its namespace

---

## Namespace Mapping

| Semantic Folder | Namespace | Purpose | Public API |
|-----------------|-----------|---------|-----------|
| **Models/** | `OfficeAutomator.Core.Models` | Data classes | Public |
| **State/** | `OfficeAutomator.Core.State` | State management | Internal |
| **Error/** | `OfficeAutomator.Core.Error` | Error handling | Internal |
| **Services/** | `OfficeAutomator.Core.Services` | Business logic | Public |
| **Validation/** | `OfficeAutomator.Core.Validation` | Configuration validation | Public |
| **Installation/** | `OfficeAutomator.Core.Installation` | Installation logic | Internal |
| **Infrastructure/** | `OfficeAutomator.Core.Infrastructure` | Dependency Injection | Internal |

---

## Usage in Production Code

### Importing Models
```csharp
using OfficeAutomator.Core.Models;

public class ConfigurationService
{
    public void ProcessConfiguration(Configuration config)
    {
        // Use Configuration class from Models namespace
    }
}
```

### Importing Services
```csharp
using OfficeAutomator.Core.Services;

public class Office
{
    private VersionSelector _versionSelector = new();
    private LanguageSelector _languageSelector = new();
    private AppExclusionSelector _appSelector = new();
}
```

### Importing Validation
```csharp
using OfficeAutomator.Core.Validation;

public class InstallationWorkflow
{
    private ConfigValidator _validator = new();
    private ConfigGenerator _generator = new();
    
    public bool Validate(Configuration config)
    {
        return _validator.Execute(config);
    }
}
```

---

## Using Statements in Tests

### Test for Models
```csharp
using OfficeAutomator.Core.Models;
using Xunit;

namespace OfficeAutomator.Core.Tests.Models
{
    public class ConfigurationTests
    {
        [Fact]
        public void Configuration_ShouldHaveValidProperties()
        {
            var config = new Configuration { Version = "2024" };
            Assert.NotNull(config);
        }
    }
}
```

### Test for Services
```csharp
using OfficeAutomator.Core.Services;
using OfficeAutomator.Core.Models;
using Xunit;

namespace OfficeAutomator.Core.Tests.Services
{
    public class VersionSelectorTests
    {
        [Fact]
        public void GetVersionOptions_ShouldReturnValidVersions()
        {
            var selector = new VersionSelector();
            var options = selector.GetVersionOptions();
            Assert.NotEmpty(options);
        }
    }
}
```

### Test for Validation
```csharp
using OfficeAutomator.Core.Validation;
using OfficeAutomator.Core.Models;
using Xunit;

namespace OfficeAutomator.Core.Tests.Validation
{
    public class ConfigValidatorTests
    {
        [Fact]
        public void Execute_WithValidConfig_ShouldPass()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { Version = "2024", Language = "es-MX" };
            
            bool result = validator.Execute(config);
            Assert.True(result);
        }
    }
}
```

### Test for Installation (with Mocking)
```csharp
using OfficeAutomator.Core.Installation;
using OfficeAutomator.Core.Infrastructure;
using Xunit;
using Moq;

namespace OfficeAutomator.Core.Tests.Installation
{
    public class InstallationExecutorTests
    {
        [Fact]
        public void Execute_WithValidSetup_ShouldStartInstallation()
        {
            var mockFileSystem = new Mock<IFileSystem>();
            var mockRegistry = new Mock<IRegistry>();
            var mockSecurityContext = new Mock<ISecurityContext>();
            var mockProcessRunner = new Mock<IProcessRunner>();
            
            var executor = new InstallationExecutor(
                mockFileSystem.Object,
                mockRegistry.Object,
                mockSecurityContext.Object,
                mockProcessRunner.Object
            );
            
            // Test logic
        }
    }
}
```

### Test for Infrastructure
```csharp
using OfficeAutomator.Core.Infrastructure;
using Xunit;

namespace OfficeAutomator.Core.Tests.Infrastructure
{
    public class DependenciesTests
    {
        [Fact]
        public void Dependencies_ShouldProvideInterfaceImplementations()
        {
            var deps = new Dependencies();
            Assert.NotNull(deps);
        }
    }
}
```

---

## Namespace Resolution Order

When referencing a class, C# resolves namespaces in this order:

1. **Local namespace** (same file)
2. **Current project** (explicit using statements)
3. **Referenced assemblies** (other projects)

Example:
```csharp
// In: tests/OfficeAutomator.Core.Tests/Models/ConfigurationTests.cs

using OfficeAutomator.Core.Models;  // ← Explicit
using Xunit;                         // ← External

public class ConfigurationTests
{
    public void Test()
    {
        var config = new Configuration();  // ← Resolved from Models
    }
}
```

---

## Public vs Internal APIs

### Public APIs (Exported in NuGet)

These are intended for external consumers:

```csharp
namespace OfficeAutomator.Core.Models
{
    public class Configuration { }  // ← Public
}

namespace OfficeAutomator.Core.Services
{
    public class VersionSelector { }     // ← Public
    public class LanguageSelector { }    // ← Public
    public class AppExclusionSelector { }// ← Public
}

namespace OfficeAutomator.Core.Validation
{
    public class ConfigValidator { }  // ← Public (used to validate configs)
}
```

### Internal APIs (Used within library)

These are for internal logic only:

```csharp
namespace OfficeAutomator.Core.State
{
    internal class OfficeAutomatorStateMachine { }  // ← Internal
}

namespace OfficeAutomator.Core.Error
{
    internal class ErrorHandler { }  // ← Internal
}

namespace OfficeAutomator.Core.Installation
{
    internal class InstallationExecutor { }  // ← Internal
    internal class RollbackExecutor { }      // ← Internal
}

namespace OfficeAutomator.Core.Infrastructure
{
    public interface IFileSystem { }      // ← Public (for DI)
    public interface IRegistry { }        // ← Public (for DI)
    public interface ISecurityContext { } // ← Public (for DI)
    public interface IProcessRunner { }   // ← Public (for DI)
}
```

---

## Namespace Best Practices

### ✓ DO

1. **Match filesystem to namespace:**
   ```
   Physical: src/OfficeAutomator.Core/Models/Configuration.cs
   Namespace: OfficeAutomator.Core.Models
   ```

2. **Use explicit using statements:**
   ```csharp
   using OfficeAutomator.Core.Models;
   using OfficeAutomator.Core.Services;
   ```

3. **Import only what you use:**
   ```csharp
   using OfficeAutomator.Core.Models;  // Used
   // Don't import everything
   ```

4. **Group usings by type:**
   ```csharp
   // System namespaces first
   using System;
   using System.Collections.Generic;
   
   // Third-party
   using Xunit;
   using Moq;
   
   // Project namespaces
   using OfficeAutomator.Core.Models;
   using OfficeAutomator.Core.Services;
   ```

### ✗ DON'T

1. **Don't create deep nested namespaces:**
   ```csharp
   // ✗ Too deep
   namespace OfficeAutomator.Core.Services.Selection.Version
   
   // ✓ Correct
   namespace OfficeAutomator.Core.Services
   ```

2. **Don't mix responsibilities in same namespace:**
   ```csharp
   // ✗ Wrong - different purposes
   namespace OfficeAutomator.Core
   {
       public class Configuration { }      // Models
       public class VersionSelector { }    // Services
       public class ErrorHandler { }       // Error
   }
   ```

3. **Don't use namespace prefixes in class names:**
   ```csharp
   // ✗ Redundant
   namespace OfficeAutomator.Core.Models
   {
       public class ModelsConfiguration { }
   }
   
   // ✓ Correct
   namespace OfficeAutomator.Core.Models
   {
       public class Configuration { }
   }
   ```

4. **Don't use global usings for everything:**
   ```csharp
   // ✗ Confusing
   global using OfficeAutomator.Core.Services;
   global using OfficeAutomator.Core.Models;
   global using OfficeAutomator.Core.Validation;
   
   // ✓ Explicit per file
   using OfficeAutomator.Core.Models;
   ```

---

## Common Namespace Mistakes & Solutions

### Mistake 1: Wrong Using Statement
```csharp
// ✗ Wrong
using OfficeAutomator.Core;  // Too generic

var selector = new VersionSelector();  // ERROR: VersionSelector not found

// ✓ Correct
using OfficeAutomator.Core.Services;

var selector = new VersionSelector();  // OK
```

### Mistake 2: Circular Dependencies
```csharp
// ✗ Wrong
namespace OfficeAutomator.Core.Models
{
    using OfficeAutomator.Core.Services;  // Models shouldn't depend on Services
}

// ✓ Correct
namespace OfficeAutomator.Core.Services
{
    using OfficeAutomator.Core.Models;  // Services depend on Models - OK
}
```

### Mistake 3: Missing Using Statement
```csharp
// ✗ Wrong
public class ConfigurationTests
{
    public void Test()
    {
        var config = new Configuration();  // ERROR: Configuration not found
    }
}

// ✓ Correct
using OfficeAutomator.Core.Models;

public class ConfigurationTests
{
    public void Test()
    {
        var config = new Configuration();  // OK
    }
}
```

---

## Namespace Refactoring

When adding new classes:

1. **Create semantic folder** (if needed)
   ```bash
   mkdir src/OfficeAutomator.Core/NewFolder/
   ```

2. **Create class file** with correct namespace
   ```csharp
   // src/OfficeAutomator.Core/NewFolder/NewClass.cs
   namespace OfficeAutomator.Core.NewFolder
   {
       public class NewClass { }
   }
   ```

3. **Create test folder** (mirror structure)
   ```bash
   mkdir tests/OfficeAutomator.Core.Tests/NewFolder/
   ```

4. **Create test file** with corresponding namespace
   ```csharp
   // tests/OfficeAutomator.Core.Tests/NewFolder/NewClassTests.cs
   using OfficeAutomator.Core.NewFolder;
   using Xunit;
   
   namespace OfficeAutomator.Core.Tests.NewFolder
   {
       public class NewClassTests { }
   }
   ```

---

## Namespace Validation

To verify namespaces match filesystem structure:

```bash
# Find mismatched namespaces
for file in src/OfficeAutomator.Core/**/*.cs; do
    folder=$(dirname "$file" | sed 's|src/OfficeAutomator.Core/||')
    expected_ns="OfficeAutomator.Core.$(basename "$folder")"
    actual_ns=$(grep "^namespace" "$file" | cut -d' ' -f2)
    
    if [ "$expected_ns" != "$actual_ns" ]; then
        echo "MISMATCH: $file"
        echo "  Expected: $expected_ns"
        echo "  Actual: $actual_ns"
    fi
done
```

---

**Version:** 1.0.0  
**Last Updated:** 2026-04-21  
**Status:** Complete

