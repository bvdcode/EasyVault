import {
  Box,
  IconButton,
  LinearProgress,
  Paper,
  Typography,
} from "@mui/material";
import { t } from "i18next";
import { VaultData } from "../types";
import { toast } from "react-toastify";
import { VaultEntryEditForm } from ".";
import { VaultApiService } from "../services";
import { confirm } from "material-ui-confirm";
import { useNavigate } from "react-router-dom";
import { useEffect, useRef, useState } from "react";
import { VaultEntryEditFormRef } from "./VaultEntryEditForm";
import { Add, Delete, Edit, Save } from "@mui/icons-material";
import { DataGrid, GridColDef, GridActionsCellItem } from "@mui/x-data-grid";

interface VaultListProps {
  password: string;
}

const VaultList: React.FC<VaultListProps> = ({ password }) => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [changed, setChanged] = useState(false);
  const editFormRef = useRef<VaultEntryEditFormRef>(null);
  const [vaultData, setVaultData] = useState<VaultData[]>([]);
  const columns: GridColDef<VaultData>[] = [
    {
      field: "appName",
      headerName: t("vaultList.columns.appName"),
      flex: 1,
      minWidth: 200,
    },
    {
      field: "secretsCount",
      headerName: t("vaultList.columns.secretsCount"),
      width: 150,
      type: "number",
      valueGetter: (_, row) => Object.keys(row.values).length,
    },
    {
      field: "actions",
      type: "actions",
      headerName: t("vaultList.columns.actions"),
      width: 120,
      getActions: (params) => [
        <GridActionsCellItem
          key="edit"
          icon={<Edit />}
          label={t("vaultList.editEntry")}
          onClick={() => handleEdit(params.row)}
          color="primary"
        />,
        <GridActionsCellItem
          key="delete"
          icon={<Delete />}
          label={t("vaultList.deleteEntry")}
          onClick={() => handleDelete(params.row)}
          color="error"
        />,
      ],
    },
  ];

  useEffect(() => {
    if (!password) {
      navigate("/login");
      return;
    }
    const fetchVaultData = async () => {
      try {
        setLoading(true);
        const data = await VaultApiService.getVaultData(password);
        setVaultData(data);
      } catch (err) {
        toast.error(
          t("vaultList.fetchError", {
            error: err instanceof Error ? err.message : String(err),
          })
        );
      } finally {
        setLoading(false);
      }
    };

    fetchVaultData();
  }, [navigate, password]);

  const getRandomString = (length: number): string => {
    const characters =
      "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    let result = "";
    for (let i = 0; i < length; i++) {
      result += characters.charAt(
        Math.floor(Math.random() * characters.length)
      );
    }
    return result;
  };

  const handleAddNewEntry = () => {
    setVaultData((prevData) => [
      ...prevData,
      {
        keyId: crypto.randomUUID(),
        appName: t("vaultList.newEntryAppName", {
          number: prevData.length + 1,
        }),
        allowedAddresses: ["172.*.*.*"],
        allowedUserAgents: ["ef", "EasyVault.Api/1.0.0"],
        values: {
          PostgresHost: "postgres-server",
          PostgresPort: "5432",
          PostgresDatabase: "",
          PostgresUsername: "",
          PostgresPassword: "",
          JwtKey: getRandomString(64),
          JwtIssuer: "app",
          JwtAudience: "client",
          SentryDsn: "abc:123@glitchtip.company.domain/1",
        },
      },
    ]);
  };

  const handleSave = async () => {
    try {
      setLoading(true);
      await VaultApiService.saveVaultData(password, vaultData);
      toast.success(t("vaultList.saveSuccess"));
      setChanged(false);
    } catch (err) {
      toast.error(
        t("vaultList.saveError", {
          error: err instanceof Error ? err.message : String(err),
        })
      );
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (item: VaultData) => {
    const itemJson = JSON.stringify(item).replace(`"apiKey":"",`, "");
    const itemCopy = JSON.parse(itemJson) as VaultData;

    confirm({
      title: t("vaultList.confirmEditTitle"),
      content: <VaultEntryEditForm ref={editFormRef} item={itemCopy} />,
      cancellationText: t("common.cancel"),
      confirmationText: t("common.confirm"),
    }).then((result) => {
      if (result.confirmed && editFormRef.current) {
        const updatedData = editFormRef.current.getFormData();
        let isDuplicateName = vaultData.some(
          (existingItem) =>
            existingItem.keyId !== updatedData.keyId &&
            existingItem.appName.toLowerCase() ===
              updatedData.appName.toLowerCase()
        );

        if (isDuplicateName) {
          let counter = 2;
          const originalName = updatedData.appName;

          while (isDuplicateName) {
            updatedData.appName = `${originalName} (${counter})`;

            isDuplicateName = vaultData.some(
              (existingItem) =>
                existingItem.keyId !== updatedData.keyId &&
                existingItem.appName.toLowerCase() ===
                  updatedData.appName.toLowerCase()
            );

            counter++;
          }

          toast.info(
            t("vaultList.nameAdjusted", { newName: updatedData.appName })
          );
        }

        const sortedValues: Record<string, string> = {};
        Object.keys(updatedData.values)
          .sort((a, b) => a.localeCompare(b))
          .forEach((key) => {
            sortedValues[key] = updatedData.values[key];
          });

        const finalData = {
          ...updatedData,
          values: sortedValues,
        };

        setVaultData((prevData) =>
          prevData.map((existingItem) =>
            existingItem.keyId === finalData.keyId ? finalData : existingItem
          )
        );

        const updatedJson = JSON.stringify(updatedData);
        const isReallyChanged = itemJson !== updatedJson;
        if (isReallyChanged) {
          setChanged(true);
          toast.warning(t("vaultList.editSuccess"));
        } else {
          toast.info(t("vaultList.noChanges"));
        }
      }
    });
  };

  const handleDelete = (item: VaultData) => {
    confirm({
      title: t("vaultList.confirmDeleteTitle"),
      description: t("vaultList.confirmDeleteDescription", {
        appName: item.appName,
      }),
      confirmationText: t("vaultList.confirmDeleteConfirm"),
      cancellationText: t("common.cancel"),
      confirmationButtonProps: { color: "error" },
    }).then((result) => {
      if (result.confirmed) {
        setVaultData((prevData) =>
          prevData.filter((i) => i.keyId !== item.keyId)
        );
        toast.success(t("vaultList.deleteSuccess"));
        setChanged(true);
      }
    });
  };

  return (
    <Paper>
      <Box
        maxWidth={800}
        margin="auto"
        padding={1}
        display="flex"
        flexDirection="column"
        alignItems="center"
      >
        <Typography variant="h6" gutterBottom>
          {t("vaultList.title", {
            keyPart:
              password?.length > 4 ? `****${password.slice(-4)}` : password,
          })}
        </Typography>
        {loading && <LinearProgress sx={{ width: "100%", mb: 2 }} />}{" "}
        {!loading && (
          <>
            {vaultData && vaultData.length > 0 ? (
              <Box sx={{ height: 631, width: "100%" }}>
                <DataGrid
                  rows={vaultData}
                  columns={columns}
                  getRowId={(row) => row.keyId}
                  initialState={{
                    pagination: {
                      paginationModel: {
                        pageSize: 10,
                      },
                    },
                  }}
                  pageSizeOptions={[5, 10, 25]}
                  checkboxSelection={false}
                  disableRowSelectionOnClick
                />
              </Box>
            ) : (
              <Typography variant="body1" sx={{ my: 3 }}>
                {t("vaultList.emptyMessage")}
              </Typography>
            )}

            <Box
              display="flex"
              flexDirection="row"
              justifyContent="space-between"
              alignItems="center"
              marginTop={2}
              gap={1}
              width="100%"
            >
              <IconButton
                title={
                  changed
                    ? t("vaultList.saveEntries")
                    : t("vaultList.noChanges")
                }
                onClick={handleSave}
                disabled={!changed}
                sx={{
                  display: vaultData.length === 0 ? "none" : "inline-flex",
                }}
              >
                <Save color={changed ? "primary" : "disabled"} />
              </IconButton>
              <Box flexGrow={1} />
              <IconButton
                title={t("vaultList.addNewEntry")}
                onClick={handleAddNewEntry}
              >
                <Add color="primary" />
              </IconButton>
            </Box>
          </>
        )}
      </Box>
    </Paper>
  );
};

export default VaultList;
