# Calc.MVVM (WPF UI) Module

Back to [Overall Orchestration](../README.md)

The `Calc.MVVM` module contains the user interface (UI) of the `Calc` application. It is built using Windows Presentation Foundation (WPF) and strictly follows the Model-View-ViewModel (MVVM) design pattern.

## The MVVM Pattern in `Calc`

The diagram below illustrates how the MVVM pattern is implemented in this project.

```mermaid
graph TD
    subgraph View (XAML)
        direction LR
        Window[CalcProjectView.xaml]
        Button
    end

    subgraph ViewModel (C#)
        direction LR
        ProjectViewModel
        SaveCommand["ICommand Save"]
    end

    subgraph Model (Calc.Core)
        direction LR
        CoreObjects[Assembly, Material, etc.]
        CoreServices[SnapshotMaker, DirectusManager]
    end

    Button --"Click Event<br/>(Binds to Command)"--> SaveCommand
    Window --"DataContext"--> ProjectViewModel
    ProjectViewModel --"Has a"--> SaveCommand
    ProjectViewModel --"Reads/Writes"--> CoreObjects
    ProjectViewModel --"Calls"--> CoreServices

    style Button fill:#f9f,stroke:#333,stroke-width:2px
```

-   The **View** (`.xaml` files) only knows about the **ViewModel**. It binds its controls to the ViewModel's properties and commands.
-   The **ViewModel** (`.cs` files) contains all the UI logic. It knows about the **Model** (`Calc.Core` objects and services), but it does not know about the View.
-   The **Model** (`Calc.Core`) is completely independent and knows nothing about the ViewModel or the View.

## Key Classes

### Views (`Views` folder)

-   **`CalcBuilderView.xaml`**: The main window for the "Calc Builder" feature.
-   **`CalcProjectView.xaml`**: The main window for the "Calc Project" feature.
-   **`LoginView.xaml`**: A window for handling user login.
-   **`AssemblySelectionView.xaml`**: A reusable view for selecting an existing assembly.
-   **`MaterialSelectionView.xaml`**: A reusable view for selecting materials.
-   **Resource Dictionaries**: `.xaml` files that define common styles for UI elements.

### ViewModels (`ViewModels` folder)

-   **`ProjectViewModel`**: The main ViewModel for the `CalcProjectView`, orchestrating the project calculation workflow.
-   **`BuilderViewModel`**: The main ViewModel for the `CalcBuilderView`, orchestrating the assembly creation workflow.
-   **`LoginViewModel`**: Manages the user login process.
-   **`QueryTemplateViewModel`**: Manages the selection and execution of `QueryTemplate`s.
-   **`MappingViewModel`**: Manages the creation, selection, and application of `Mapping`s.
-   **`MappingErrorViewModel`**: Manages the display of elements that could not be mapped.
-   **`NodeTreeViewModel`**: Manages the hierarchical tree view of queried elements.
-   **`CalculationViewModel`**: Manages the calculation results and error reporting.
-   **`AssemblySelectionViewModel`**: Provides the logic for the `AssemblySelectionView`.
-   **`MaterialSelectionViewModel`**: Provides the logic for the `MaterialSelectionView`.
-   **`AssemblyCreationViewModel`**: Manages the state for creating a new assembly.
-   **`SavingViewModel`**: Handles the logic for the "Save Snapshot" dialog.
-   **`VisibilityViewModel`**: Controls the visibility of overlay panels (e.g., "Waiting...").

### Models (`Models` folder)

These are UI-specific data wrappers that simplify data from `Calc.Core` for display in the View.

-   **`NodeModel`**: Wraps a `Branch` or `Query` from `Core` for display in the tree view.
-   **`AssemblyModel`**: Represents the assembly information for a selected `NodeModel`.
-   **`LayerMaterialModel`**: Wraps a `Material` for display in an assembly layer.
-   **`AmountModel`**: Represents the quantity take-offs for a selection.
-   **`FilterTagModel`**: Represents a single filter criterion in the UI.
-   **`StandardModel`**: Represents a single LCA standard in a list.
-   **`CategorizedResultModel`**: A model for displaying results grouped by category.
