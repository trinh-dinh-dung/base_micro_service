import vi from '../i18n/locales/vi.json';
import en from '../i18n/locales/en.json';

export interface AuditMetadataItem {
  fieldName: string;
  labelVi: string;
  labelEn: string;
  translationKey: string;
}

function getPageMetadata(localeObject: unknown, pageCode: string): Record<string, string> {
  const root = localeObject as {
    auditMetadata?: Record<string, Record<string, string>>;
  };

  return root.auditMetadata?.[pageCode] ?? {};
}

export function getAuditMetadataByPage(pageCode: string): AuditMetadataItem[] {
  const viMap = getPageMetadata(vi, pageCode);
  const enMap = getPageMetadata(en, pageCode);
  const fieldNames = Array.from(new Set([...Object.keys(viMap), ...Object.keys(enMap)])).sort();

  return fieldNames.map((fieldName) => ({
    fieldName,
    labelVi: viMap[fieldName] ?? fieldName,
    labelEn: enMap[fieldName] ?? viMap[fieldName] ?? fieldName,
    translationKey: `auditMetadata.${pageCode}.${fieldName}`,
  }));
}
