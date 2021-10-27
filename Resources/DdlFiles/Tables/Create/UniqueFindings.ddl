CREATE TABLE IF NOT EXISTS UniqueFindings
(
    UniqueFinding_ID              INTEGER PRIMARY KEY,
    InstanceIdentifier            NVARCHAR(50),
    ToolGeneratedOutput           NVARCHAR,
    Comments                      NVARCHAR,
    FindingDetails                NVARCHAR,
    FirstDiscovered               DATETIME     NOT NULL,
    LastObserved                  DATETIME     NOT NULL,
    DeltaAnalysisIsRequired       NVARCHAR(5)  NOT NULL,
    FindingType_ID                INTEGER      NOT NULL,
    FindingSourceFile_ID          INTEGER      NOT NULL,
    Status                        NVARCHAR(25) NOT NULL,
    Vulnerability_ID              INTEGER      NOT NULL,
    VulnerabilitySource_ID        INTEGER      NOT NULL,
    Hardware_ID                   INTEGER,
    Software_ID                   INTEGER,
    SeverityOverride              NVARCHAR(25),
    SeverityOverrideJustification NVARCHAR(2000),
    TechnologyArea                NVARCHAR(100),
    WebDB_Site                    NVARCHAR(500),
    WebDB_Instance                NVARCHAR(100),
    Classification                NVARCHAR(25),
    CVSS_EnvironmentalScore       NVARCHAR(5),
    CVSS_EnvironmentalVector      NVARCHAR(25),
    FOREIGN KEY (FindingType_ID) REFERENCES FindingTypes (FindingType_ID),
    FOREIGN KEY (Vulnerability_ID) REFERENCES Vulnerabilities (Vulnerability_ID),
    FOREIGN KEY (VulnerabilitySource_ID) REFERENCES VulnerabilitySources (VulnerabilitySource_ID),
    FOREIGN KEY (Hardware_ID) REFERENCES Hardware (Hardware_ID),
    FOREIGN KEY (Software_ID) REFERENCES Software (Software_ID),
    FOREIGN KEY (FindingSourceFile_ID) REFERENCES UniqueFindingsSourceFiles (FindingSourceFile_ID),
    UNIQUE (
            InstanceIdentifier,
            Hardware_ID,
            Vulnerability_ID,
            VulnerabilitySource_ID,
            FindingType_ID
        ) ON CONFLICT IGNORE,
    UNIQUE (
            InstanceIdentifier,
            Software_ID,
            Vulnerability_ID,
            VulnerabilitySource_ID,
            FindingType_ID
        ) ON CONFLICT IGNORE
);