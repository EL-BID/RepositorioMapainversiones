
namespace IMRepo
{
    public class ModelsTexts
    {

        static public class Addition
        {
            public const string title = "Adición";
            public const string titlePlural = "Adiciones";
            public const string description = "Las adiciones al producto registran cada una de las modificaciones al costo total programado del producto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id adición producto";
            public const string CodeTitle = "Código";
            public const string CodeDescription = "Código de la Adición. Asignado automáticamente por el sistema. Se usa para facilitar la búsqueda o identificación de la Adición al costo del producto.";
            public const string ProductTitle = "Producto";
            public const string ProductDescription = "Producto al que se le hace una adición en el costo planeado.";
            public const string ValueTitle = "Costo Adicional";
            public const string ValueDescription = "Valor que se adiciona al costo del producto.";
            public const string StageTitle = "Etapa";
            public const string StageDescription = "Etapa en la que se encuentra la Adición (presentada o aprobada)";
            public const string DateDeliveryTitle = "Fecha presentación";
            public const string DateDeliveryDescription = "Fecha en la que se presento el tramite de la Adición para que se apruebe.";
            public const string DateApprovedTitle = "Fecha aprobación";
            public const string DateApprovedDescription = "Fecha de aprobación de la Adición.";
            public const string NotesTitle = "Notas";
            public const string NotesDescription = "Comentarios o anotaciones que explican o justifican la Adición al costo del producto.";
            public const string AttachmentTitle = "Soporte de aprobación";
            public const string AttachmentDescription = "Archivo adjunto que contiene el documento donde se aprueba la Adición al costo planeado del producto.";
            public const string AttachmentsTitle = "Otros anexos";
            public const string AttachmentsDescription = "Archivos anexos que soportan la Adición";
        }

        static public class AdditionAttachment
        {
            public const string title = "Anexo Adición";
            public const string titlePlural = "Anexos Adición";
            public const string description = "Cada uno de los archivos anexos de una Adición al costo del producto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id Anexo Adición";
            public const string AdditionTitle = "Adición";
            public const string AdditionDescription = "Adición al costo del producto a la que corresponde el anexo.";
            public const string TitleTitle = "Título";
            public const string TitleDescription = "Título que identifica el documento anexo a la Adición.";
            public const string FileNameTitle = "Archivo";
            public const string FileNameDescription = "Nombre del archivo que contiene el anexo.";
            public const string DateAttachedTitle = "Fecha de registro";
            public const string DateAttachedDescription = "Fecha en la que se carga el documento anexo en el sistema.";
        }

        static public class ExtensionAttachment
        {
            public const string title = "Anexo Extensión";
            public const string titlePlural = "Anexos Extensión";
            public const string description = "Cada uno de los archivos anexos de Extensión de tiempo del proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id Anexo Extensión";
            public const string ExtensionTitle = "Extensión";
            public const string ExtensionDescription = "Extensión a la que corresponde el anexo.";
            public const string TitleTitle = "Título";
            public const string TitleDescription = "Título que identifica el documento o anexo cargado.";
            public const string FileNameTitle = "Archivo";
            public const string FileNameDescription = "Archivo que se anexa a la Extensión de tiempo del proyecto.";
            public const string DateAttachedTitle = "Fecha de carga";
            public const string DateAttachedDescription = "Fecha en la que se carga el anexo en el sistema.";
        }

        static public class PaymentAttachment
        {
            public const string title = "Anexo Pago";
            public const string titlePlural = "Anexos Pago";
            public const string description = "Cada uno de los archivos anexos de un pago.";
            public const string IdTitle = "id";
            public const string IdDescription = "id Anexo Pago";
            public const string PaymentTitle = "Pago";
            public const string PaymentDescription = "El Pago al que pertenece el documento anexo.";
            public const string TitleTitle = "Título";
            public const string TitleDescription = "Título que explica el contenido del documento anexo.";
            public const string FileTitle = "Archivo";
            public const string FileDescription = "Archivo donde se almacena el anexo";
            public const string DateAttachedTitle = "Fecha de carga";
            public const string DateAttachedDescription = "Fecha en que se carga el documento en el sistema.";
        }

        static public class ProjectAttachment
        {
            public const string title = "Anexo Proyecto";
            public const string titlePlural = "Anexos Proyecto";
            public const string description = "Cada uno de los archivos anexos a un proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id Anexo Proyecto";
            public const string ProjectTitle = "Proyecto";
            public const string ProjectDescription = "Proyecto al que pertenece el archivo anexo.";
            public const string TitleTitle = "Título";
            public const string TitleDescription = "Título que indica el contenido del anexo";
            public const string FileNameTitle = "Archivo";
            public const string FileNameDescription = "Nombre del archivo que contiene el anexo.";
            public const string DateAttachedTitle = "Fecha de carga";
            public const string DateAttachedDescription = "Fecha en que se carga el documento en el sistema.";
        }

        static public class Agency
        {
            public const string title = "Entidad Ejecutora";
            public const string titlePlural = "Entidades Ejecutoras";
            public const string description = "Lista de cada uno de las posibles entidades ejecutoras.";
            public const string IdTitle = "id";
            public const string IdDescription = "Id entidad ejecutora";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre o razón social de la entidad.";
            public const string AcronymTitle = "Sigla";
            public const string AcronymDescription = "La sigla o abreviatura que permite identificar la entidad ejecutora con un texto breve. Para ser utilizada en reportes.";
            public const string OfficialIDTitle = "# ID";
            public const string OfficialIDDescription = "Número de identificación oficial de la entidad ejecutora.";
        }

        static public class FundingAgency
        {
            public const string title = "Entidad Financiadora";
            public const string titlePlural = "Entidades Financiadoras";
            public const string description = "Cada una de las posibles fuentes de financiación de los proyectos.";
            public const string IdTitle = "id";
            public const string IdDescription = "id agencia de financiamiento";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre que identifica a la agencia de financiamiento o la entidad que financia.";
            public const string AcronymTitle = "Sigla";
            public const string AcronymDescription = "La sigla o abreviatura que permite identificar la entidad financiadora con un texto breve. Para ser utilizada en reportes.";
        }

        static public class PaymentStage
        {
            public const string title = "Etapa de Pago";
            public const string titlePlural = "Etapas de Pago";
            public const string description = "Cada uno de las etapas en que puede encontrarse un pago (presentado, aprobado, pagado)";
            public const string IdTitle = "ID";
            public const string IdDescription = "ID estado del pago";
            public const string TitleTitle = "Título";
            public const string TitleDescription = "Título que identifica al estado de pago.";
            public const string SortOrderTitle = "Orden";
            public const string SortOrderDescription = "Número que indica la posición del estado de pago en una lista de estados.";
        }

        static public class TaskStage
        {
            public const string title = "Etapa de Trámite";
            public const string titlePlural = "Etapas de Trámite";
            public const string description = "Posibles etapas de un trámite como una Extensión. (presentada, aprobada)";
            public const string IdTitle = "id";
            public const string IdDescription = "id estado trámite";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre o título que identifica al estado.";
            public const string OrderTitle = "Orden";
            public const string OrderDescription = "El orden en que se presenta el estado dentro de la lista de estados de trámites.";
        }

        static public class ProjectStage
        {
            public const string title = "Etapa Proyecto";
            public const string titlePlural = "Etapas Proyecto";
            public const string description = "Las posibles etapas en que puede encontrarse un proyecto (planeación, ejecución, finalizado)";
            public const string IdTitle = "id";
            public const string IdDescription = "id de la etapa del proyecto";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre o título que identifica la etapa del proyecto.";
            public const string OrderTitle = "Orden";
            public const string OrderDescription = "Número que indica el orden en el que se presenta la opción dentro de la lista de etapas del proyecto.";
        }

        static public class Extension
        {
            public const string title = "Extensión";
            public const string titlePlural = "Extensiones";
            public const string description = "Extensión de la duración del proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id extension";
            public const string CodeTitle = "Código";
            public const string CodeDescription = "Código de la Extensión asignado automáticamente por el sistema. Se usa para que los usuarios puedan buscar o identificar la Extensión de tiempo de ejecución del proyecto.";
            public const string ProjectTitle = "Proyecto";
            public const string ProjectDescription = "Proyecto al que aplica la extensión.";
            public const string DaysTitle = "Días de extensión";
            public const string DaysDescription = "TIempo en días en el que se extiende el plazo del proyecto";
            public const string StageTitle = "Etapa";
            public const string StageDescription = "La Etapa en que se encuentra la solicitud de extensión de tiempo (presentada, aprobada)";
            public const string DateDeliveryTitle = "Fecha presentación";
            public const string DateDeliveryDescription = "Fecha en la que se presentó el trámite de la Extensión de plazo para su aprobación.";
            public const string DateApprovedTitle = "Fecha aprobación";
            public const string DateApprovedDescription = "Fecha de aprobación de la Extensión.";
            public const string MotiveTitle = "Observación";
            public const string MotiveDescription = "Motivo por el cual se hace la Extensión de tiempo o de plazo del proyecto.";
            public const string AttachmentTitle = "Acto de aprobación";
            public const string AttachmentDescription = "Archivo adjunto que contiene el documento en el que se soporta y aprueba la Extensión de plazo del proyecto.";
            public const string AttachmentsTitle = "Anexos";
            public const string AttachmentsDescription = "Permite administrar los anexos para la Extensión.";
        }

        static public class ProjectFunding
        {
            public const string title = "Fuente de Financiamiento";
            public const string titlePlural = "Fuentes de Financiamiento";
            public const string description = "Cada una de las fuentes de financiamiento del proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id project source";
            public const string ProjectTitle = "Proyecto";
            public const string ProjectDescription = "Proyecto al que corresponde la fuente de financiamiento.";
            public const string TypeTitle = "Tipo de Financiamiento";
            public const string TypeDescription = "El tipo de financiamiento correspondiente al monto indicado para la entidad. (Pretamo, recursos propios, donación, APP)";
            public const string SourceTitle = "Fuente";
            public const string SourceDescription = "Organísmo o entidad que financia el monto indicado.";
            public const string ValueTitle = "Valor";
            public const string ValueDescription = "Monto o valor a ser financiado por la fuente.";
        }

        static public class ProjectImage
        {
            public const string title = "Imagen";
            public const string titlePlural = "Imágenes";
            public const string description = "El sistema permite cargar imágenes que muestren el estado del proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id imagen del proyecto";
            public const string ProjectTitle = "Proyecto";
            public const string ProjectDescription = "Proyecto al que corresponde la imagen.";
            public const string FileTitle = "Archivo";
            public const string FileDescription = "Nombre del archivo que contiene la imagen.";
            public const string DescriptionTitle = "Descripción";
            public const string DescriptionDescription = "Descripción de la imagen.";
            public const string ImageDateTitle = "Fecha";
            public const string ImageDateDescription = "Fecha en la que produjo la imagen.";
            public const string UploadDateTitle = "Fecha de cargue";
            public const string UploadDateDescription = "Fecha en que se registra la imagen en el sistema.";
        }

        static public class Sdg
        {
            public const string title = "Objetivo de Desarrollo Sostenible";
            public const string titlePlural = "Objetivos de Desarrollo Sostenible";
            public const string description = "Cada uno de los objetivos de desarrollo sostenible de la ONU";
            public const string IdTitle = "ID";
            public const string IdDescription = "SDG ID";
            public const string NumberTitle = "#";
            public const string NumberDescription = "Número de ODS";
            public const string TitleTitle = "Nombre";
            public const string TitleDescription = "Nombre del ODS";
        }

        static public class Office
        {
            public const string title = "Oficina";
            public const string titlePlural = "Oficinas";
            public const string description = "Cada una de las oficinas que puede ser responsable de un proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id oficina";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre de la oficina o área de la administración de la entidad gubernamental.";
        }

        static public class Payment
        {
            public const string title = "Pago";
            public const string titlePlural = "Pagos";
            public const string description = "Certificados de pago.";
            public const string IdTitle = "id";
            public const string IdDescription = "id Pago";
            public const string CodeTitle = "Código";
            public const string CodeDescription = "Código de Certificado asignado automáticamente por el sistema. Se usa para facilitar la identificación y consulta del certificado.";
            public const string ProductTitle = "Producto";
            public const string ProductDescription = "Producto al que corresponde el pago realizado y el avance registrado";
            public const string FundingSourceTitle = "Fuente de Financiamiento";
            public const string FundingSourceDescription = "La fuente de financiamiento de la que se obtienen los recursos para el pago.";
            public const string ReportedMonthTitle = "Mes de Medición";
            public const string ReportedMonthDescription = "Periodo (mes) de medición del avance. Se toman el mes y el año de la fecha.";
            public const string PaymentAmountTitle = "Monto";
            public const string PaymentAmountDescription = "Valor o monto del pago.";
            public const string PhysicalAdvanceTitle = "Avance físico (%)";
            public const string PhysicalAdvanceDescription = "El avance fisico reportado en el certificado.";
            public const string StageTitle = "Etapa";
            public const string StageDescription = "La etapa en la que se encuentra el Certificado de Pago. (Presentado, aprobado, pagado)";
            public const string DateDeliveryTitle = "Fecha presentación";
            public const string DateDeliveryDescription = "Fecha del presentación del certificado";
            public const string DateApprovedTitle = "Fecha aprobación";
            public const string DateApprovedDescription = "Fecha en que el certificado fue aprobado o en que el valor correspondiente es devengado.";
            public const string DatePayedTitle = "Fecha pagado";
            public const string DatePayedDescription = "Fecha de pago del certificado.";
            public const string AttachmentAdvanceTitle = "Soporte de avance";
            public const string AttachmentAdvanceDescription = "Arvchivo anexo que contiene el documento que soporta el avance reportado.";
            public const string AttachmentPaymentTitle = "Soporte de pago";
            public const string AttachmentPaymentDescription = "Archivo anexo que contiene el documento que soporta el pago realizado.";
        }

        static public class UserProfile
        {
            public const string title = "Perfil de Usuario";
            public const string titlePlural = "Perfiles de Usuario";
            public const string description = "Información de registro del usuario en el sistema.";
            public const string IdTitle = "id";
            public const string IdDescription = "Id perfil de usuario";
            public const string AspNetUserIdTitle = "AspNet User Id";
            public const string AspNetUserIdDescription = "El id interno de Asp Net para el usuario.";
            public const string EmailTitle = "Correo electrónico";
            public const string EmailDescription = "Correo Electrónico del usuario";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre del usuario";
            public const string SurnameTitle = "Apellido";
            public const string SurnameDescription = "Apellido del usuario";
            public const string OfficeTitle = "Oficina";
            public const string OfficeDescription = "Oficina o área de la administración de la entidad gubernamental a la que pertenece el usuario";
            public const string NotesTitle = "Notas";
            public const string NotesDescription = "Anotaciones o comentarios";
        }

        static public class Product
        {
            public const string title = "Producto";
            public const string titlePlural = "Productos";
            public const string description = "Cada uno de los productos de un proyecto";
            public const string IdTitle = "ID";
            public const string IdDescription = "ID producto";
            public const string ProjectTitle = "Proyecto";
            public const string ProjectDescription = "Proyecto al que pertenece el producto.";
            public const string NameTitle = "Nombre del Producto";
            public const string NameDescription = "El nombre que identifica al producto del proyecto.";
            public const string CostTitle = "Costo Estimado";
            public const string CostDescription = "Costo estimado inicial del producto";
            public const string DescriptionTitle = "Descripción";
            public const string DescriptionDescription = "Descripción del producto";
            public const string ObjectiveTitle = "Objetivo Específico";
            public const string ObjectiveDescription = "Objetivo Específico al que aporta el producto.";
        }

        static public class Project
        {
            public const string title = "Proyecto";
            public const string titlePlural = "Proyectos";
            public const string description = "Información general del proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id proyecto";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre del proyecto";
            public const string CodeTitle = "Código";
            public const string CodeDescription = "Código que identifica al proyecto";
            public const string SectorTitle = "Sector";
            public const string SectorDescription = "Principal sector al que aporta la ejecución del proyecto.";
            public const string SubsectorTitle = "Subsector";
            public const string SubsectorDescription = "Subsector dentro del cual se desarrolla el proyecto.";
            public const string OfficeTitle = "Área responsable";
            public const string OfficeDescription = "Oficina, organismo, entidad o área de la administración del ente gubernamental responsable del proyecto.";
            public const string ExecutingAgencyTitle = "Entidad Ejecutora";
            public const string ExecutingAgencyDescription = "Entidad que ejecuta el proyecto.";
            public const string StageTitle = "Etapa";
            public const string StageDescription = "Estado en que se encuentra el proyecto.";
            public const string SdgTitle = "ODS";
            public const string SdgDescription = "Objetivo de Desarrollo Sostenible al que contribuye el proyecto con mayor relevancia.";
            public const string PlannedCostTitle = "Costo estimado";
            public const string PlannedCostDescription = "Costo estimado del proyecto";
            public const string PlannedDurationTitle = "Duración estimada";
            public const string PlannedDurationDescription = "Duración en días estimada del proyecto.";
            public const string PlannedStartDateTitle = "Fecha de Inicio planeada";
            public const string PlannedStartDateDescription = "Fecha estimada de inicio del proyecto.";
            public const string ActualStartDateTitle = "Fecha de Inicio Real";
            public const string ActualStartDateDescription = "Fecha real de inicio del proyecto.";
            public const string ActualEndDateTitle = "Fecha de Finalización";
            public const string ActualEndDateDescription = "Fecha real de finalización del proyecto.";
            public const string DescriptionTitle = "Descripción";
            public const string DescriptionDescription = "Descripción del proyecto";
            public const string ObjectivesTitle = "Objetivos";
            public const string ObjectivesDescription = "Objetivos del Proyecto";
            public const string LocationTitle = "Ubicación";
            public const string LocationDescription = "Ubiación georreferenciada del proyecto.";
            public const string FuentesTitle = "Fuentes de financiamiento";
            public const string FuentesDescription = "Fuentes de financiamiento del proyecto.";
            public const string AttachmentsTitle = "Anexos";
            public const string AttachmentsDescription = "Archivos anexos al proyecto.";
        }

        static public class Sector
        {
            public const string title = "Sector";
            public const string titlePlural = "Sectores";
            public const string description = "Principal sector al que aporta el proyecto";
            public const string IdTitle = "id";
            public const string IdDescription = "id sector";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre o título que identifica al sector";
        }

        static public class Subsector
        {
            public const string title = "Subsector";
            public const string titlePlural = "Subsectores";
            public const string description = "Subsector o subgrupo al que aporta el proyecto";
            public const string IdTitle = "id";
            public const string IdDescription = "id subsector";
            public const string SectorTitle = "Sector";
            public const string SectorDescription = "Sector al que pertenece el subsector.";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Nombre o título que identifica al subsector.";
        }

        static public class FundingType
        {
            public const string title = "Tipo de Financiamiento";
            public const string titlePlural = "Tipos de Financiamiento";
            public const string description = "Cada uno de los diferentes tipos de financiamiento posibles (ej. Prestamo, Donación, Recursos Propios, APP)";
            public const string IdTitle = "id";
            public const string IdDescription = "id tipo financiamiento";
            public const string NameTitle = "Nombre";
            public const string NameDescription = "Título o nombre que identifica al tipo de financiamiento.";
        }

        static public class ProjectVideo
        {
            public const string title = "Video";
            public const string titlePlural = "Videos";
            public const string description = "El sistema permite crear vinculos externos a videos publicados relativos al proyecto.";
            public const string IdTitle = "id";
            public const string IdDescription = "id Video";
            public const string ProjectTitle = "Proyecto";
            public const string ProjectDescription = "Proyecto al que corresponde el video";
            public const string LinkTitle = "Enlace";
            public const string LinkDescription = "Enlace que permite acceder al video. El video puede publicarse en una plataforma como YouTube e incluir aquí el enlace correspondiente.";
            public const string DescriptionTitle = "Descripción";
            public const string DescriptionDescription = "Descripción del video";
            public const string VideoDateTitle = "Fecha del video";
            public const string VideoDateDescription = "Fecha en que se produjo el video";
            public const string UploadDateTitle = "Fecha de cargue";
            public const string UploadDateDescription = "Fecha en que se registra el video en el sistema.";
        }
    }
}

