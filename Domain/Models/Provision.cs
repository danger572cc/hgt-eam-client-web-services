using System.Globalization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HGT.EAM.Client.Domain.Models;

public class Provision
{
    [Key]
    public long Id { get; set; }

    [JsonPropertyName("ctac_nombre")] public string? DescripcionCuenta { get; set; }
    [JsonPropertyName("clase_activo")] public string? ClaseActivo { get; set; }
    [JsonPropertyName("mat_desc")] public string? MatDesc { get; set; }
    [JsonPropertyName("movi")] public string? DescripcionMovimiento { get; set; }
    [JsonPropertyName("padre1")] public string? Padre1 { get; set; }
    [JsonPropertyName("padre2")] public string? Padre2 { get; set; }
    [JsonPropertyName("padre3")] public string? Padre3 { get; set; }
    [JsonPropertyName("tipo_padre1")] public string? TipoPadre1 { get; set; }
    [JsonPropertyName("tipo_padre2")] public string? TipoPadre2 { get; set; }
    [JsonPropertyName("tipo_padre3")] public string? TipoPadre3 { get; set; }
    [JsonPropertyName("wic_folio")] public string? Folio { get; set; }
    [JsonPropertyName("agrupamiento")] public string? AgrupamientoOT { get; set; }
    [JsonPropertyName("cco_desc")] public string? DescripcionCentroCosto { get; set; }
    [JsonPropertyName("ceco")] public string? CentroCosto { get; set; }
    [JsonPropertyName("cim_desc")] public string? DescripcionCImp { get; set; }
    [JsonPropertyName("cls_desc")] public string? DescripcionClaseOT { get; set; }
    [JsonPropertyName("com_class")] public string? ClaseProveedor { get; set; }
    [JsonPropertyName("com_code")] public string? Proveedor { get; set; }
    [JsonPropertyName("com_desc")] public string? DescripcionProveedor { get; set; }
    [JsonPropertyName("cta_fxr")] public string? CuentaPorPagar { get; set; }
    [JsonPropertyName("ctac")] public string? CuentaContable { get; set; }
    [JsonPropertyName("deporder")] public string? DepartamentoOC { get; set; }
    [JsonPropertyName("depot")] public string? DepartamentoOT { get; set; }
    [JsonPropertyName("dis_cimputacion")] public string? CostoImputacion { get; set; }
    [JsonPropertyName("dis_porcentaje")] public decimal? PorcentajeDistribucion { get; set; }
    [JsonPropertyName("dis_valor")] public decimal? TotalEnvio { get; set; }
    [JsonPropertyName("distribuido")] public decimal? ValorDistribucion { get; set; }
    [JsonPropertyName("evt_class")] public string? ClaseOT { get; set; }
    [JsonPropertyName("evt_failureusage")] public decimal? LecturaHorometro { get; set; }
    [JsonPropertyName("evt_jobtype")] public string? TipoTrabajo { get; set; }
    [JsonPropertyName("evt_jobtype_desc")] public string? DescripcionTrabajo { get; set; }
    [JsonPropertyName("evt_object")] public string? Equipo { get; set; }
    [JsonPropertyName("evt_object_org")] public string? OrganizacionEquipo { get; set; }
    [JsonPropertyName("evt_obtype")] public string? TipoOB { get; set; }
    [JsonPropertyName("evt_obtype_desc")] public string? TipoOBDescripcion { get; set; }
    [JsonPropertyName("evt_org")] public string? OrganizacionOT { get; set; }
    [JsonPropertyName("evt_status")] public string? Estado { get; set; }
    [JsonPropertyName("evt_status_desc")] public string? DescripcionEstado { get; set; }
    [JsonPropertyName("obj_desc")] public string? DescripcionEquipo { get; set; }
    [JsonPropertyName("obj_parent")] public string? Padre { get; set; }
    [JsonPropertyName("obj_parent_desc")] public string? DescripcionPadre { get; set; }
    [JsonPropertyName("obj_parent_org")] public string? OrganizacionPadre { get; set; }
    [JsonPropertyName("ord_class")] public string? ClaseOC { get; set; }
    [JsonPropertyName("ord_desc")] public string? DescripcionOC { get; set; }
    [JsonPropertyName("org_curr")] public string? MonedaEnvio { get; set; }
    [JsonPropertyName("orl_curr")] public string? MonedaOC { get; set; }
    [JsonPropertyName("orl_exch")] public decimal? TasaCambio { get; set; }
    [JsonPropertyName("orl_order_org")] public string? OrganizacionOC { get; set; }
    [JsonPropertyName("orl_ordqty")] public decimal? Cantidad { get; set; }
    [JsonPropertyName("orl_price")] public decimal? Precio { get; set; }
    [JsonPropertyName("orl_recvqty")] public decimal? CantidadRecibida { get; set; }
    [JsonPropertyName("par_class")] public string? ClasePieza { get; set; }
    [JsonPropertyName("par_desc")] public string? DescripcionPieza { get; set; }
    [JsonPropertyName("par_uom")] public string? UnidadMedida { get; set; }
    [JsonPropertyName("pco_periodo")] public decimal? PcoPeriodo { get; set; }
    [JsonPropertyName("precio_enviar")] public decimal? PrecioEnviar { get; set; }
    [JsonPropertyName("price_por_exch")] public decimal? PrecioConTasaCambio { get; set; }
    [JsonPropertyName("store")] public string? Almacen { get; set; }
    [JsonPropertyName("tra_org")] public string? Organizacion { get; set; }
    [JsonPropertyName("trl_date")] public DateTime? FechaTransaccion { get; set; }
    [JsonPropertyName("trl_event")] public string? OrdenTrabajo { get; set; }
    [JsonPropertyName("trl_line")] public decimal? LineaTransaccion { get; set; }
    [JsonPropertyName("trl_order")] public string? OrdenCompra { get; set; }
    [JsonPropertyName("trl_ordline")] public decimal? LineaTransaccionOC { get; set; }
    [JsonPropertyName("trl_part")] public string? PiezaTransaccion { get; set; }
    [JsonPropertyName("trl_poextracharges")] public decimal? PoExtraCharges { get; set; }
    [JsonPropertyName("trl_price")] public decimal? PrecioTransaccion { get; set; }
    [JsonPropertyName("trl_qty")] public decimal? CantidadTransaccion { get; set; }
    [JsonPropertyName("trl_trans")] public string? Transaccion { get; set; }
    [JsonPropertyName("trl_type")] public string? Tipo { get; set; }
    [JsonPropertyName("wic_code")] public string? WicCode { get; set; }
    [JsonPropertyName("wic_correlativo")] public decimal? NumeroEnvio { get; set; }
    [JsonPropertyName("wic_error")] public string? Error { get; set; }
    [JsonPropertyName("wic_fecha")] public DateTime? Fecha { get; set; }
    [JsonPropertyName("wic_mensaje")] public string? MensajeEnvio { get; set; }
    [JsonPropertyName("wic_status")] public decimal? EstadoEnvio { get; set; }
}

public class ProvisionConfiguration : IEntityTypeConfiguration<Provision>
{
    public void Configure(EntityTypeBuilder<Provision> b)
    {
        b.ToTable("Provisiones");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();

        b.Property(x => x.OrganizacionOT).HasMaxLength(180);

        b.HasIndex(x => x.Organizacion);
        b.HasIndex(x => x.OrdenCompra);

        b.Property(x => x.FechaTransaccion);
        b.Property(x => x.Fecha);

        b.Property(x => x.LineaTransaccion).HasPrecision(10, 0);
        b.Property(x => x.LineaTransaccionOC).HasPrecision(10, 0);
        b.Property(x => x.PorcentajeDistribucion).HasPrecision(20, 4);
        b.Property(x => x.ValorDistribucion).HasPrecision(20, 2);
        b.Property(x => x.Cantidad).HasPrecision(20, 6);
        b.Property(x => x.CantidadRecibida).HasPrecision(20, 6);
        b.Property(x => x.TasaCambio).HasPrecision(20, 2);
        b.Property(x => x.Precio).HasPrecision(20, 2);
        b.Property(x => x.PrecioTransaccion).HasPrecision(20, 2);
        b.Property(x => x.PrecioConTasaCambio).HasPrecision(20, 2);
        b.Property(x => x.TotalEnvio).HasPrecision(20, 4);
        b.Property(x => x.Cantidad).HasPrecision(20, 4);
        b.Property(x => x.LecturaHorometro).HasPrecision(20, 2);
        b.Property(x => x.CantidadTransaccion).HasPrecision(20, 4);
        b.Property(x => x.NumeroEnvio).HasPrecision(20, 2);
        b.Property(x => x.EstadoEnvio).HasPrecision(20, 2);
        b.Property(x => x.PcoPeriodo).HasPrecision(20, 2);
        b.Property(x => x.PrecioEnviar).HasPrecision(20, 2);
        b.Property(x => x.PoExtraCharges).HasPrecision(20, 2);

        b.Property(x => x.Organizacion).HasMaxLength(50);
        b.Property(x => x.DescripcionProveedor).HasMaxLength(200);
        b.Property(x => x.ClaseProveedor).HasMaxLength(50);
        b.Property(x => x.Tipo).HasMaxLength(50);
        b.Property(x => x.Transaccion).HasMaxLength(50);
        b.Property(x => x.Almacen).HasMaxLength(50);
        b.Property(x => x.ClaseOC).HasMaxLength(50);
        b.Property(x => x.OrdenCompra).HasMaxLength(50);
        b.Property(x => x.DescripcionOC).HasMaxLength(50);
        b.Property(x => x.ClasePieza).HasMaxLength(50);
        b.Property(x => x.DescripcionPieza).HasMaxLength(50);
        b.Property(x => x.CuentaContable).HasMaxLength(50);
        b.Property(x => x.CentroCosto).HasMaxLength(50);
        b.Property(x => x.CostoImputacion).HasMaxLength(50);
        b.Property(x => x.CuentaPorPagar).HasMaxLength(50);
        b.Property(x => x.MonedaEnvio).HasMaxLength(50);
        b.Property(x => x.UnidadMedida).HasMaxLength(50);
        b.Property(x => x.MonedaOC).HasMaxLength(50);
        b.Property(x => x.OrdenTrabajo).HasMaxLength(50);
        b.Property(x => x.TipoOB).HasMaxLength(200);
        b.Property(x => x.DescripcionTrabajo).HasMaxLength(200);
        b.Property(x => x.TipoTrabajo).HasMaxLength(50);
        b.Property(x => x.TipoOBDescripcion).HasMaxLength(200);
        b.Property(x => x.Equipo).HasMaxLength(50);
        b.Property(x => x.DescripcionEquipo).HasMaxLength(200);
        b.Property(x => x.Padre).HasMaxLength(50);
        b.Property(x => x.DescripcionPadre).HasMaxLength(200);
        b.Property(x => x.ClaseOT).HasMaxLength(50);
        b.Property(x => x.DescripcionClaseOT).HasMaxLength(200);
        b.Property(x => x.Estado).HasMaxLength(50);
        b.Property(x => x.DescripcionEstado).HasMaxLength(200);
        b.Property(x => x.PiezaTransaccion).HasMaxLength(50);
        b.Property(x => x.ClaseActivo).HasMaxLength(50);
        b.Property(x => x.Padre1).HasMaxLength(50);
        b.Property(x => x.TipoPadre1).HasMaxLength(50);
        b.Property(x => x.Padre2).HasMaxLength(50);
        b.Property(x => x.TipoPadre2).HasMaxLength(50);
        b.Property(x => x.Padre3).HasMaxLength(50);
        b.Property(x => x.TipoPadre3).HasMaxLength(50);
        b.Property(x => x.DepartamentoOC).HasMaxLength(50);
        b.Property(x => x.DepartamentoOT).HasMaxLength(50);
        b.Property(x => x.Proveedor).HasMaxLength(50);
        b.Property(x => x.Folio).HasMaxLength(50);
        b.Property(x => x.Error).HasMaxLength(50);
        b.Property(x => x.MensajeEnvio).HasMaxLength(200);
        b.Property(x => x.DescripcionCuenta).HasMaxLength(200);
        b.Property(x => x.MatDesc).HasMaxLength(50);
        b.Property(x => x.DescripcionMovimiento).HasMaxLength(200);
        b.Property(x => x.OrganizacionPadre).HasMaxLength(200);
        b.Property(x => x.OrganizacionEquipo).HasMaxLength(200);
        b.Property(x => x.AgrupamientoOT).HasMaxLength(50);
        b.Property(x => x.DescripcionCentroCosto).HasMaxLength(200);
        b.Property(x => x.DescripcionCImp).HasMaxLength(50);
        b.Property(x => x.OrganizacionOC).HasMaxLength(50);
        b.Property(x => x.WicCode).HasMaxLength(50);
    }
}

public static class ProvisionMapper
{
    public static (List<Provision> Items, int TotalRecords, int TotalPages) FromApiEnvelope(string json, CultureInfo? culture = null)
    {
        using var doc = JsonDocument.Parse(json);

        int totalPages = 0;
        int totalRecords = 0;
        var items = new List<Provision>();

        if (doc.RootElement.TryGetProperty("data", out var data))
        {
            if (data.TryGetProperty("totalPages", out var tp) && tp.ValueKind == JsonValueKind.Number)
                totalPages = tp.GetInt32();

            if (data.TryGetProperty("totalRecords", out var tr) && tr.ValueKind == JsonValueKind.Number)
                totalRecords = tr.GetInt32();

            if (data.TryGetProperty("dataRecord", out var record) &&
                record.TryGetProperty("rows", out var rows))
            {
                var raw = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(rows.GetRawText())
                        ?? new List<Dictionary<string, JsonElement>>();

                var list = raw.Select(item =>
                    item.ToDictionary(
                        kv => kv.Key,
                        kv => MapperHelper.GetSafeString(kv.Value)
                    )
                ).ToList();

                items = list.Select(row => MapFromAliasDict(row, culture)).ToList();
            }
        }

        return (items, totalRecords, totalPages);
    }

    private static Provision MapFromAliasDict(Dictionary<string, string> row, CultureInfo? culture)
    {
        string Get(string key) => row.TryGetValue(key, out var v) ? v : string.Empty;

        var model = new Provision
        {
            DescripcionCuenta = Get("ctac_nombre") ?? "",
            ClaseActivo = Get("clase_activo") ?? "",
            MatDesc = Get("mat_desc") ?? "",
            DescripcionMovimiento = Get("movi") ?? "",
            Padre1 = Get("padre1") ?? "",
            Padre2 = Get("padre2") ?? "",
            Padre3 = Get("padre3") ?? "",
            TipoPadre1 = Get("tipo_padre1") ?? "",
            TipoPadre2 = Get("tipo_padre2") ?? "",
            TipoPadre3 = Get("tipo_padre3") ?? "",
            Folio = Get("wic_folio") ?? "",
            AgrupamientoOT = Get("agrupamiento") ?? "",
            DescripcionCentroCosto = Get("cco_desc") ?? "",
            CentroCosto = Get("ceco") ?? "",
            DescripcionCImp = Get("cim_desc") ?? "",
            DescripcionClaseOT = Get("cls_desc") ?? "",
            ClaseProveedor = Get("com_class") ?? "",
            Proveedor = Get("com_code") ?? "",
            DescripcionProveedor = Get("com_desc") ?? "",
            CuentaPorPagar = Get("cta_fxr") ?? "",
            CuentaContable = Get("ctac") ?? "",
            DepartamentoOC = Get("deporder") ?? "",
            DepartamentoOT = Get("depot") ?? "",
            CostoImputacion = Get("dis_cimputacion") ?? "",
            PorcentajeDistribucion = MapperHelper.ParseDecimal(Get("dis_porcentaje"), culture),
            TotalEnvio = MapperHelper.ParseDecimal(Get("dis_valor"), culture),
            ValorDistribucion = MapperHelper.ParseDecimal(Get("distribuido"), culture),
            ClaseOT = Get("evt_class") ?? "",
            LecturaHorometro = MapperHelper.ParseDecimal(Get("evt_failureusage"), culture),
            TipoTrabajo = Get("evt_jobtype") ?? "",
            DescripcionTrabajo = Get("evt_jobtype_desc") ?? "",
            Equipo = Get("evt_object") ?? "",
            OrganizacionEquipo = Get("evt_object_org") ?? "",
            TipoOB = Get("evt_obtype") ?? "",
            TipoOBDescripcion = Get("evt_obtype_desc") ?? "",
            OrganizacionOT = Get("evt_org") ?? "",
            Estado = Get("evt_status") ?? "",
            DescripcionEstado = Get("evt_status_desc") ?? "",
            DescripcionEquipo = Get("obj_desc") ?? "",
            Padre = Get("obj_parent") ?? "",
            DescripcionPadre = Get("obj_parent_desc") ?? "",
            OrganizacionPadre = Get("obj_parent_org") ?? "",
            ClaseOC = Get("ord_class") ?? "",
            DescripcionOC = Get("ord_desc") ?? "",
            MonedaEnvio = Get("org_curr") ?? "",
            MonedaOC = Get("orl_curr") ?? "",
            TasaCambio = MapperHelper.ParseDecimal(Get("orl_exch"), culture),
            OrganizacionOC = Get("orl_order_org") ?? "",
            Cantidad = MapperHelper.ParseDecimal(Get("orl_ordqty"), culture),
            Precio = MapperHelper.ParseDecimal(Get("orl_price"), culture),
            CantidadRecibida = MapperHelper.ParseDecimal(Get("orl_recvqty"), culture),
            ClasePieza = Get("par_class") ?? "",
            DescripcionPieza = Get("par_desc") ?? "",
            UnidadMedida = Get("par_uom") ?? "",
            PcoPeriodo = MapperHelper.ParseDecimal(Get("pco_periodo"), culture),
            PrecioEnviar = MapperHelper.ParseDecimal(Get("precio_enviar"), culture),
            PrecioConTasaCambio = MapperHelper.ParseDecimal(Get("price_por_exch"), culture),
            Almacen = Get("store") ?? "",
            Organizacion = Get("tra_org") ?? "",
            FechaTransaccion = MapperHelper.ParseDate(Get("trl_date"), culture),
            OrdenTrabajo = Get("trl_event") ?? "",
            LineaTransaccion = MapperHelper.ParseLong(Get("trl_line"), culture),
            OrdenCompra = Get("trl_order") ?? "",
            LineaTransaccionOC = MapperHelper.ParseLong(Get("trl_ordline"), culture),
            PiezaTransaccion = Get("trl_part") ?? "",
            PoExtraCharges = MapperHelper.ParseDecimal(Get("trl_poextracharges"), culture),
            PrecioTransaccion = MapperHelper.ParseDecimal(Get("trl_price"), culture),
            CantidadTransaccion = MapperHelper.ParseDecimal(Get("trl_qty"), culture),
            Transaccion = Get("trl_trans") ?? "",
            Tipo = Get("trl_type") ?? "",
            WicCode = Get("wic_code") ?? "",
            NumeroEnvio = MapperHelper.ParseLong(Get("wic_correlativo"), culture),
            Error = Get("wic_error") ?? "",
            Fecha = MapperHelper.ParseDate(Get("wic_fecha"), culture),
            MensajeEnvio = Get("wic_mensaje") ?? "",
            EstadoEnvio = MapperHelper.ParseLong(Get("wic_status"), culture)
        };

        return model;
    }
}