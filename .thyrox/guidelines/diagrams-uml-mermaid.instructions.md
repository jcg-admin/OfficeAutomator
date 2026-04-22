```yml
type: Guía de Desarrollo
subject: UML Diagrams with Mermaid
version: 1.0.0
purpose: Define standards for UML and flowchart diagrams in OfficeAutomator
applies_to: All documentation requiring visual architecture
updated_at: 2026-04-22 16:45:00
```

# GUÍA DE DESARROLLO — UML & MERMAID DIAGRAMS

**Visual Architecture Documentation for OfficeAutomator**

---

## TABLE OF CONTENTS

1. [Philosophy](#philosophy)
2. [Core Requirements](#core-requirements)
3. [Mermaid Configuration](#mermaid-configuration)
4. [Three-Layer Architecture Diagrams](#three-layer-architecture-diagrams)
5. [Class Diagrams (C#)](#class-diagrams-c)
6. [Flowchart Diagrams](#flowchart-diagrams)
7. [Sequence Diagrams](#sequence-diagrams)
8. [State Machine Diagrams](#state-machine-diagrams)
9. [Color Standards](#color-standards)
10. [Anti-Patterns in Diagrams](#anti-patterns-in-diagrams)
11. [Quality Checklist](#quality-checklist)

---

## PHILOSOPHY

### Core Principle

**Diagrams document architecture clearly. No decoration, no emojis, no symbols. Substance only.**

Mermaid diagrams serve as executable documentation. They must be:

```
THREE PILLARS:

1. CLARITY
   - Labels are precise and specific
   - Structure reflects actual architecture
   - No ambiguous connections

2. READABILITY
   - Dark theme for accessibility
   - Readable color contrast
   - Logical node ordering

3. MAINTAINABILITY
   - Diagram matches code structure
   - Updates reflect real changes
   - No decorative elements
```

### Absolutely Prohibited

- Emojis of any kind: NO ✓, ❌, ⚠, 🔄, 📊, ⏳, etc.
- Decorative icons or symbols
- ASCII art in labels
- Unnecessary styling
- Color used for decoration (only for semantic meaning)

---

## CORE REQUIREMENTS

### Requirement 1: Dark Theme Mandatory

Every diagram MUST include:

```mermaid
%%{init: { 'theme': 'dark' } }%%
```

This is non-negotiable. Light theme diagrams are not acceptable.

### Requirement 2: No Emojis, Ever

Examples of PROHIBITED syntax:

```
PROHIBITED: "✓ Validated"
PROHIBITED: "❌ Error"
PROHIBITED: "🔄 Retry"
PROHIBITED: "📁 File"
PROHIBITED: "⚠ Warning"
PROHIBITED: "SUCCESS ✅"
```

Examples of CORRECT syntax:

```
CORRECT: "[VALIDATED] Configuration"
CORRECT: "[ERROR] Validation failed"
CORRECT: "[RETRY] Max retries exceeded"
CORRECT: "[FILE] Configuration.xml"
CORRECT: "[WARNING] Deprecated"
CORRECT: "[SUCCESS] Completed"
```

### Requirement 3: Semantic Labels Only

Every node MUST have a clear, descriptive label.

```
INCORRECT: "V1" (what is V1?)
INCORRECT: "P" (what is P?)
INCORRECT: "E" (what is E?)

CORRECT: "[VALIDATE] Configuration"
CORRECT: "[PROCESS] User Input"
CORRECT: "[ERROR] Invalid Format"
```

### Requirement 4: Color Semantics

Colors convey meaning, not decoration:

- **Blue** = Start/End or infrastructure
- **Green** = Success, validation passed, OK
- **Red** = Error, blocked, failure
- **Orange/Yellow** = Warning, retry, special handling
- **Gray** = Neutral, informational

---

## MERMAID CONFIGURATION

### Standard Initialization

```mermaid
%%{init: { 'theme': 'dark', 'logLevel': 'error' } }%%
```

Parameters explained:
- `'theme': 'dark'` = Use dark background (required)
- `'logLevel': 'error'` = Suppress non-critical logs

### Never Use

- Light theme
- `'layoutDirection': 'auto'` (specify explicitly)
- `'primaryColor'` in init (use style blocks)
- `'fontSize'` overrides (maintain standard sizing)

---

## THREE-LAYER ARCHITECTURE DIAGRAMS

### Standard Three-Layer View

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph LR
    Layer0["[LAYER 0]<br/>Bootstrap<br/>(Bash)"]
    Layer1["[LAYER 1]<br/>Orchestration<br/>(PowerShell)"]
    Layer2["[LAYER 2]<br/>Core Logic<br/>(C#)"]
    
    Layer0 -->|Prepares| Layer1
    Layer1 -->|Invokes| Layer2
    
    style Layer0 fill:#1e3a3a,stroke:#4da6ff,stroke-width:3px,color:#fff
    style Layer1 fill:#3a3a1e,stroke:#ffd54f,stroke-width:3px,color:#fff
    style Layer2 fill:#1e1e3a,stroke:#9966ff,stroke-width:3px,color:#fff
```

### Layer 0: Bootstrap Scripts

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([SYSTEM START]) --> SetupBash["[SCRIPT] setup.sh"]
    Start --> VerifyBash["[SCRIPT] verify-environment.sh"]
    
    SetupBash --> ValidateBash{"[VALIDATE]<br/>SDK installed?"}
    VerifyBash --> CheckBash{"[CHECK]<br/>Requirements met?"}
    
    ValidateBash -->|Yes| ReadyLayer1["[READY] Layer 1 execution"]
    ValidateBash -->|No| ErrorSetup["[ERROR] Setup failed"]
    
    CheckBash -->|Yes| ReadyLayer1
    CheckBash -->|No| ErrorVerify["[ERROR] Verification failed"]
    
    ReadyLayer1 --> TransferControl["[HAND OFF] To PowerShell"]
    
    ErrorSetup --> ExitFail1["[EXIT] 1"]
    ErrorVerify --> ExitFail2["[EXIT] 1"]
    TransferControl --> ExitSuccess["[EXIT] 0"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style SetupBash fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style VerifyBash fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style ValidateBash fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style CheckBash fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style ReadyLayer1 fill:#1a3a1a,stroke:#81c784,stroke-width:2px,color:#fff
    style TransferControl fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style ErrorSetup fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style ErrorVerify fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style ExitFail1 fill:#5d0d0d,stroke:#ef5350,stroke-width:2px,color:#fff
    style ExitFail2 fill:#5d0d0d,stroke:#ef5350,stroke-width:2px,color:#fff
    style ExitSuccess fill:#0d5d0d,stroke:#66bb6a,stroke-width:2px,color:#fff
```

### Layer 1: PowerShell Orchestration (UC Flow)

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([INVOKE ORCHESTRATOR]) --> UC1["[UC-001] Select Version"]
    UC1 --> UC2["[UC-002] Select Language"]
    UC2 --> UC3["[UC-003] Exclude Applications"]
    
    UC3 --> GenXML["[GENERATE] Configuration.xml"]
    GenXML --> DL["[DOWNLOAD] Office Deployment Tool"]
    
    DL --> UC4["[UC-004] Validate Integrity"]
    UC4 -->|Valid| UC5["[UC-005] Install Office"]
    UC4 -->|Invalid| ErrorValidate["[BLOCKED] Validation failed"]
    
    UC5 -->|Success| Complete["[COMPLETED] Installation done"]
    UC5 -->|Failure| ErrorInstall["[ERROR] Installation failed"]
    
    ErrorValidate --> End1["[EXIT] 1"]
    ErrorInstall --> End2["[EXIT] 1"]
    Complete --> End3["[EXIT] 0"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style UC1 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style UC2 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style UC3 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style GenXML fill:#2d4a2b,stroke:#ffd54f,stroke-width:2px,color:#fff
    style DL fill:#2d4a2b,stroke:#ffd54f,stroke-width:2px,color:#fff
    style UC4 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style UC5 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style ErrorValidate fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style ErrorInstall fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Complete fill:#1a3a1a,stroke:#81c784,stroke-width:2px,color:#fff
    style End1 fill:#5d0d0d,stroke:#ef5350,stroke-width:2px,color:#fff
    style End2 fill:#5d0d0d,stroke:#ef5350,stroke-width:2px,color:#fff
    style End3 fill:#0d5d0d,stroke:#66bb6a,stroke-width:2px,color:#fff
```

---

## CLASS DIAGRAMS (C#)

### Standard Class Diagram Format

```mermaid
%%{init: { 'theme': 'dark' } }%%
classDiagram
    class OfficeAutomatorStateMachine {
        -stateStack: Stack~State~
        +Initialize() void
        +TransitionTo(state: State) bool
        +GetCurrentState() State
        +IsValidTransition(target: State) bool
    }
    
    class State {
        <<abstract>>
        #name: string
        +Enter() void
        +Exit() void
        +OnInput(input: string) State
    }
    
    class ValidationState {
        -validator: IConfigValidator
        +Enter() void
        +OnInput(input: string) State
    }
    
    class InstallationState {
        -installer: IOfficeInstaller
        +Enter() void
        +OnInput(input: string) State
    }
    
    OfficeAutomatorStateMachine --> State
    ValidationState --|> State
    InstallationState --|> State
```

### Layered Architecture Class View

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TB
    subgraph Layer2 ["[LAYER 2] C# Core Logic"]
        Models["Models<br/>(Configuration, Office)"]
        Services["Services<br/>(Validator, Installer)"]
        StateMachine["State Machine<br/>(FSM)"]
    end
    
    subgraph Layer1 ["[LAYER 1] PowerShell"]
        Orchestrator["Invoke-OfficeAutomator<br/>(UC Flow)"]
    end
    
    subgraph Layer0 ["[LAYER 0] Bash"]
        Bootstrap["setup.sh<br/>verify-environment.sh"]
    end
    
    Bootstrap -->|Validates| Layer1
    Orchestrator -->|Calls| Services
    Services -->|Uses| Models
    Services -->|Uses| StateMachine
    
    style Layer2 fill:#1e1e3a,stroke:#9966ff,stroke-width:2px,color:#fff
    style Layer1 fill:#3a3a1e,stroke:#ffd54f,stroke-width:2px,color:#fff
    style Layer0 fill:#1e3a3a,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Models fill:#2d2d4a,stroke:#7755ff,color:#fff
    style Services fill:#2d2d4a,stroke:#7755ff,color:#fff
    style StateMachine fill:#2d2d4a,stroke:#7755ff,color:#fff
    style Orchestrator fill:#4a4a2d,stroke:#ffcc44,color:#fff
    style Bootstrap fill:#2d4a4a,stroke:#44ccff,color:#fff
```

---

## FLOWCHART DIAGRAMS

### UC-004 Validation Flowchart (Complete Example)

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([VALIDATION START]) --> Phase1["[PHASE 1] Structure Validation"]
    
    Phase1 --> S1["[STEP] XML well-formed?"]
    Phase1 --> S2["[STEP] Version exists?"]
    Phase1 --> S5["[STEP] Applications available?"]
    
    S1 -->|No| E1["[BLOCKED] XML Invalid"]
    S1 -->|Yes| Sync1["[SYNC] Phase 1 complete"]
    
    S2 -->|No| E2["[BLOCKED] Version not found"]
    S2 -->|Yes| Sync1
    
    S5 -->|No| E5["[BLOCKED] Application unavailable"]
    S5 -->|Yes| Sync1
    
    Sync1 --> Phase2["[PHASE 2] Logic Validation"]
    
    Phase2 --> S3["[STEP] Language exists?"]
    S3 -->|No| E3["[BLOCKED] Language not found"]
    S3 -->|Yes| S4["[STEP] Language supported in version?"]
    
    S4 -->|No| E4["[BLOCKED] Language not supported"]
    S4 -->|Yes| S6["[STEP] Language-App combo valid?"]
    
    S6 -->|No| E6["[CRITICAL] Combo invalid - Microsoft bug prevention"]
    S6 -->|Yes| Phase3["[PHASE 3] Integrity Validation"]
    
    Phase3 --> S7["[STEP] SHA256 check"]
    S7 -->|Failed| Retry["[RETRY] Download (max 3)"]
    Retry -->|Attempt 1,2,3| S7
    Retry -->|Exhausted| E7["[RECOVERABLE] Download corrupted"]
    
    S7 -->|OK| S8["[STEP] XML executable?"]
    S8 -->|No| E8["[BLOCKED] XML not executable"]
    S8 -->|Yes| Success["[SUCCESS] Validation passed"]
    
    E1 --> EndFail["[END] Validation failed - 1"]
    E2 --> EndFail
    E3 --> EndFail
    E4 --> EndFail
    E5 --> EndFail
    E6 --> EndFail
    E7 --> EndFail
    E8 --> EndFail
    
    Success --> EndSuccess["[END] Validation passed - 0"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Phase1 fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Phase2 fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Phase3 fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style S1 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style S2 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style S3 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style S4 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style S5 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style S6 fill:#2d4a2b,stroke:#ff9800,stroke-width:2px,color:#fff
    style S7 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style S8 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style E1 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style E2 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style E3 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style E4 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style E5 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style E6 fill:#5d2c1a,stroke:#ff6f00,stroke-width:2px,color:#fff
    style E7 fill:#3d2a1a,stroke:#ffa726,stroke-width:2px,color:#fff
    style E8 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Sync1 fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Retry fill:#2d3a1a,stroke:#ffd54f,stroke-width:2px,color:#fff
    style Success fill:#1a3a1a,stroke:#81c784,stroke-width:2px,color:#fff
    style EndSuccess fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style EndFail fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

---

## SEQUENCE DIAGRAMS

### UC-005 Installation Sequence

```mermaid
%%{init: { 'theme': 'dark' } }%%
sequenceDiagram
    participant User
    participant PowerShell as [Layer 1] PowerShell
    participant Service as [Layer 2] Service
    participant Installer as [Layer 2] Installer
    participant System as [System] Windows
    
    User->>PowerShell: Invoke-OfficeAutomator
    PowerShell->>Service: ValidateConfiguration(config)
    Service-->>PowerShell: ValidationResult (PASS)
    
    PowerShell->>Installer: Install(config)
    Installer->>System: setup.exe /configure
    System->>System: Install Office
    System-->>Installer: Exit code 0
    
    Installer-->>PowerShell: InstallationResult (SUCCESS)
    PowerShell-->>User: Installation complete
```

---

## STATE MACHINE DIAGRAMS

### OfficeAutomatorStateMachine (11-State FSM)

```mermaid
%%{init: { 'theme': 'dark' } }%%
stateDiagram-v2
    [*] --> INIT
    
    INIT --> VERSION_SELECTION: SelectVersion()
    VERSION_SELECTION --> LANGUAGE_SELECTION: VersionSelected
    LANGUAGE_SELECTION --> APP_EXCLUSION: LanguageSelected
    APP_EXCLUSION --> CONFIG_GENERATION: AppExclusionsSet
    
    CONFIG_GENERATION --> ODT_DOWNLOAD: ConfigGenerated
    ODT_DOWNLOAD --> VALIDATION: ODTDownloaded
    
    VALIDATION --> INSTALLATION: ValidationPassed
    VALIDATION --> ERROR_INVALID: ValidationFailed
    
    INSTALLATION --> CLEANUP: InstallationCompleted
    CLEANUP --> [*]: CleanupDone
    
    ERROR_INVALID --> [*]: ErrorState
    
    note right of VALIDATION
        UC-004: Exhaustive validation
        before installation
    end note
    
    note right of INSTALLATION
        UC-005: Execute setup.exe
        with configuration.xml
    end note
```

---

## COLOR STANDARDS

### Semantic Color Palette (Dark Theme)

| Usage | Background | Stroke | Text | Hex Values |
|-------|-----------|--------|------|-----------|
| **Start/End** | Gray-800 | Blue | White | `#1e1e1e` / `#4da6ff` |
| **Phase/Section** | Green-900 | Light Green | White | `#2d5016` / `#90ee90` |
| **Step OK** | Green-950 | Green | White | `#1a3a1a` / `#66bb6a` |
| **Special Handling** | Green-Brown | Orange | White | `#2d4a2b` / `#ff9800` |
| **Error Blocked** | Red-950 | Red | White | `#3d1f1a` / `#ef5350` |
| **Error Critical** | Red-Brown | Orange-Dark | White | `#5d2c1a` / `#ff6f00` |
| **Error Recoverable** | Brown-Dark | Orange-Light | White | `#3d2a1a` / `#ffa726` |
| **Retry/Loop** | Green-Gray | Yellow | White | `#2d3a1a` / `#ffd54f` |
| **Success** | Green-950 | Light Green | White | `#1a3a1a` / `#81c784` |
| **Success End** | Green-Intense | Green-Light | White | `#0d5d0d` / `#66bb6a` |
| **Failure End** | Red-Intense | Red-Light | White | `#5d0d0d` / `#ef5350` |

### Do NOT Use

- Light backgrounds
- Low-contrast combinations
- Colors for purely decorative purposes
- More than 6 distinct colors per diagram

---

## ANTI-PATTERNS IN DIAGRAMS

### Pattern 1: Emojis and Symbols (STRICTLY PROHIBITED)

```
PROHIBITED:
    ✓ Configuration validated
    ❌ Configuration invalid
    🔄 Retrying
    📁 File
    ⚠ Warning

CORRECT:
    [VALIDATED] Configuration passed
    [BLOCKED] Configuration invalid
    [RETRY] Attempting again
    [FILE] Configuration.xml
    [WARNING] Deprecated
```

### Pattern 2: Unclear Node Labels

```
INCORRECT: "V", "P", "E", "U1", "U2"
CORRECT: "[VALIDATE]", "[PROCESS]", "[ERROR]", "[UC-001]", "[UC-002]"
```

### Pattern 3: Excessive Styling

```
INCORRECT: 
    Multiple node colors for visual appeal
    Different stroke styles for decoration
    Shadow effects or gradients

CORRECT:
    Colors convey semantic meaning only
    All strokes same width unless emphasizing
    No decorative visual effects
```

### Pattern 4: Ambiguous Connections

```
INCORRECT: Node A connects to Node B, but no label on edge
CORRECT: Node A --[Description]-> Node B
```

### Pattern 5: Missing Semantic Information

```
INCORRECT:
    graph showing dataflow without error paths
    FSM without showing all states
    Sequence diagram without system boundaries

CORRECT:
    All failure paths included
    All states explicitly named
    Participants clearly labeled with layer
```

---

## QUALITY CHECKLIST

### Before Adding to Documentation

Diagram must satisfy ALL:

- [ ] Dark theme present: `%%{init: { 'theme': 'dark' } }%%`
- [ ] NO emojis or decorative symbols anywhere
- [ ] Every node has semantic label (e.g., [VALIDATED], [ERROR])
- [ ] Colors match semantic meanings (blue=start, green=ok, red=error)
- [ ] Color contrast is readable on dark background
- [ ] Text is in English (UPPERCASE or Title Case)
- [ ] All nodes have clear, descriptive labels
- [ ] Connections have labels explaining relationships
- [ ] Error paths are shown (no hidden failures)
- [ ] Diagram reflects actual architecture/flow in code
- [ ] No unnecessary nodes or edges
- [ ] All terminology matches code/documentation

### Rendering Check

Before committing:

1. Render the diagram in markdown preview
2. Verify colors are visible on dark background
3. Confirm all text is readable
4. Check that no emojis appear
5. Verify diagram layout is logical (top-to-bottom or left-to-right)

---

## EXAMPLES: BEFORE AND AFTER

### Example 1: Prohibited Emojis

BEFORE (INCORRECT):
```mermaid
graph TD
    Start(["🟢 START"]) --> Step1["✓ Validate"]
    Step1 -->|OK| Step2["🔄 Process"]
    Step2 -->|ERROR| Error["❌ Failed"]
    Error --> End(["🔴 END"])
```

AFTER (CORRECT):
```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([START]) --> Step1["[VALIDATE] Configuration"]
    Step1 -->|OK| Step2["[PROCESS] User Input"]
    Step2 -->|ERROR| Error["[ERROR] Processing failed"]
    Error --> End([END])
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Step1 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Step2 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Error fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style End fill:#5d0d0d,stroke:#ef5350,stroke-width:2px,color:#fff
```

### Example 2: Unclear Labels

BEFORE (INCORRECT):
```mermaid
graph TD
    A["S1"] --> B["P"]
    B --> C["V"]
    C -->|OK| D["I"]
    C -->|ERROR| E["E"]
```

AFTER (CORRECT):
```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    A["[SELECT] Version"] --> B["[PROCESS] User input"]
    B --> C["[VALIDATE] Configuration"]
    C -->|Valid| D["[INSTALL] Office"]
    C -->|Invalid| E["[ERROR] Validation failed"]
    
    style A fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style B fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style C fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style D fill:#1a3a1a,stroke:#81c784,color:#fff
    style E fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
```

---

## BASH SCRIPT DIAGRAMS

### Standard Bash Script Flowchart

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([SCRIPT START]) --> Check1{"[CHECK]<br/>Bash version<br/>correct?"}
    
    Check1 -->|No| Error1["[ERROR] Bash 4.0 required"]
    Check1 -->|Yes| Check2{"[CHECK]<br/>Dependencies<br/>installed?"}
    
    Check2 -->|No| Error2["[ERROR] Missing dependency"]
    Check2 -->|Yes| Init["[INITIALIZE] Variables"]
    
    Init --> Main["[EXECUTE] Main logic"]
    Main -->|Success| Cleanup["[CLEANUP] Temp files"]
    Main -->|Failure| ErrorMain["[ERROR] Main failed"]
    
    Cleanup --> Success["[SUCCESS] Script done"]
    ErrorMain --> EndFail["[END] Exit 1"]
    Error1 --> EndFail
    Error2 --> EndFail
    Success --> EndSuccess["[END] Exit 0"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Check1 fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Check2 fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Init fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Main fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Cleanup fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Success fill:#1a3a1a,stroke:#81c784,stroke-width:2px,color:#fff
    style Error1 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Error2 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style ErrorMain fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style EndSuccess fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style EndFail fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

### Bash Function Call Sequence

```mermaid
%%{init: { 'theme': 'dark' } }%%
sequenceDiagram
    participant Script as setup.sh
    participant Validator as validate_environment()
    participant Downloader as download_file()
    participant Verifier as verify_file_integrity()
    
    Script->>Validator: [CALL] Check prerequisites
    Validator->>Validator: [CHECK] Bash version
    Validator->>Validator: [CHECK] Dependencies
    Validator-->>Script: [RETURN] 0
    
    Script->>Downloader: [CALL] Get ODT
    Downloader->>Downloader: [DOWNLOAD] curl
    Downloader-->>Script: [RETURN] 0
    
    Script->>Verifier: [CALL] Validate download
    Verifier->>Verifier: [COMPUTE] SHA256
    Verifier->>Verifier: [COMPARE] Hash
    Verifier-->>Script: [RETURN] 0
    
    Script-->>Script: [SUCCESS] All checks passed
```

### Bash Error Handling Pattern

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([FUNCTION START]) --> Input{"[VALIDATE]<br/>Input<br/>provided?"}
    
    Input -->|No| ArgError["[ERROR] Missing argument"]
    Input -->|Yes| FileCheck{"[CHECK]<br/>File<br/>exists?"}
    
    FileCheck -->|No| FileError["[ERROR] File not found"]
    FileCheck -->|Yes| ReadCheck{"[CHECK]<br/>Readable?"}
    
    ReadCheck -->|No| ReadError["[ERROR] File not readable"]
    ReadCheck -->|Yes| Process["[PROCESS] Main logic"]
    
    Process -->|Success| Return0["[RETURN] Exit 0"]
    Process -->|Failure| ProcessError["[ERROR] Processing failed"]
    
    ArgError --> EndFail["[END] Return 1"]
    FileError --> EndFail
    ReadError --> EndFail
    ProcessError --> EndFail
    Return0 --> EndSuccess["[END] Return 0"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Input fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style FileCheck fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style ReadCheck fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Process fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style ArgError fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style FileError fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style ReadError fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style ProcessError fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Return0 fill:#1a3a1a,stroke:#81c784,stroke-width:2px,color:#fff
    style EndSuccess fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style EndFail fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

### Bash Retry Loop Pattern

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([RETRY LOOP]) --> Init["[INITIALIZE] Retry counter = 0"]
    Init --> Loop{"[LOOP]<br/>Retries<br/>remaining?"}
    
    Loop -->|No more retries| Exhausted["[EXHAUSTED] Max retries"]
    Loop -->|Yes| Attempt["[ATTEMPT] Execute operation"]
    
    Attempt -->|Success| Success["[SUCCESS] Operation done"]
    Attempt -->|Failure| Increment["[INCREMENT] Retry counter"]
    
    Increment --> Wait["[WAIT] 2 seconds"]
    Wait --> Loop
    
    Success --> Return0["[RETURN] Exit 0"]
    Exhausted --> Return1["[RETURN] Exit 1"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Init fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Loop fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Attempt fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Increment fill:#2d3a1a,stroke:#ffd54f,stroke-width:2px,color:#fff
    style Wait fill:#2d3a1a,stroke:#ffd54f,stroke-width:2px,color:#fff
    style Success fill:#1a3a1a,stroke:#81c784,stroke-width:2px,color:#fff
    style Exhausted fill:#3d2a1a,stroke:#ffa726,stroke-width:2px,color:#fff
    style Return0 fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style Return1 fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

---

## TOOLS AND INTEGRATION

### Markdown Integration

Mermaid diagrams render directly in markdown:

```markdown
\`\`\`mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    A --> B
\`\`\`
```

### Online Editor

Test diagrams: https://mermaid.live/

**Important:** Switch theme to "dark" in editor before finalizing.

### IDE Support

- VS Code: Markdown Preview Enhanced extension
- GitHub: Native rendering (select dark theme in settings)

---

## ARCHITECTURE DOCUMENTATION STANDARDS

### Documenting Three-Layer Architecture

Every guide or specification should include:

1. **Layer Diagram** — Shows 3 layers and data flow
2. **Layer 0 Detail** — Bootstrap scripts and validation
3. **Layer 1 Detail** — UC flow and orchestration
4. **Layer 2 Detail** — C# classes and state machine

Example structure in docs:
```
README.md
├── Architecture Overview (three-layer diagram)
├── Layer 0: Bootstrap (Bash scripts, flowchart)
├── Layer 1: Orchestration (UC sequence diagram)
└── Layer 2: Core Logic (class diagram, state machine)
```

---

**Versión:** 1.0.0
**Última actualización:** 2026-04-22
**Aplicable a:** OfficeAutomator v1.0.0+

**Fundamental Principle:**

Diagrams are part of the documentation system. They must be clear, accurate, and maintainable. No decoration, no emojis, no symbols. Substance only. Colors and labels carry semantic meaning.
