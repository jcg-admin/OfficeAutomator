.PHONY: help setup test clean

help:
	@echo "OfficeAutomator - Available targets:"
	@echo "  make setup    - Install .NET SDK 8.0 and verify environment"
	@echo "  make test     - Run all tests"
	@echo "  make clean    - Remove build artifacts"

setup: verify-env install-sdk
	@echo "✓ Setup complete"

verify-env:
	@bash ./verify-environment.sh

install-sdk:
	@bash ./setup.sh

test:
	@echo "Running tests..."
	@cd src/OfficeAutomator.Core && dotnet test

clean:
	@echo "Cleaning artifacts..."
	@rm -rf src/OfficeAutomator.Core/bin
	@rm -rf src/OfficeAutomator.Core/obj
	@echo "✓ Cleanup complete"
