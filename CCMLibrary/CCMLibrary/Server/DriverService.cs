using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCMLibrary.Enums;

namespace CCMLibrary
{
    internal class DriverService
    {
        /// <summary>
        /// Class initiate DecisionTable object
        /// hold server conditions
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static DecisionTable<ServerPhase, ClientRequest>GetServerDriver(BrokerServer instance)
        {
            var decisionTable = new DecisionTable<ServerPhase, ClientRequest>(
                Enum.GetValues(typeof(ServerPhase)).Cast<ServerPhase>(),
                Enum.GetValues(typeof(ClientRequest)).Cast<ClientRequest>()
            );

#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Idle, ClientRequest.EnrollProject, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Idle, ClientRequest.CancelEnrollment, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Idle, ClientRequest.TaskRequest, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Idle, ClientRequest.TaskResultsReturnRequest, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Idle, ClientRequest.HearbeatConfirmation, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.

#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.SendingInvitation, ClientRequest.EnrollProject, delegate (object ob1, object ob2) { return instance.ProjectInfoData(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.SendingInvitation, ClientRequest.CancelEnrollment, delegate (object ob1, object ob2) { return instance.EnrollmentCanceled(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.SendingInvitation, ClientRequest.TaskRequest, delegate (object ob1, object ob2) { return instance.Wait(ob1, ob2); });
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.SendingInvitation, ClientRequest.TaskResultsReturnRequest, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.SendingInvitation, ClientRequest.HearbeatConfirmation, delegate (object ob1, object ob2) { return instance.HearbeatNoticed(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.

#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.InProgress, ClientRequest.EnrollProject, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.InProgress, ClientRequest.CancelEnrollment, delegate (object ob1, object ob2) { return instance.EnrollmentCanceled(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.InProgress, ClientRequest.TaskRequest, delegate (object ob1, object ob2) { return instance.NodeTaskData(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.InProgress, ClientRequest.TaskResultsReturnRequest, delegate (object ob1, object ob2) { return instance.NodeTaskDataNext(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.InProgress, ClientRequest.HearbeatConfirmation, delegate (object ob1, object ob2) { return instance.HearbeatNoticed(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.

#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Cancelation, ClientRequest.EnrollProject, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Cancelation, ClientRequest.CancelEnrollment, delegate (object ob1, object ob2) { return instance.EnrollmentCanceled(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Cancelation, ClientRequest.TaskRequest, delegate (object ob1, object ob2) { return instance.Poison(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Cancelation, ClientRequest.TaskResultsReturnRequest, delegate (object ob1, object ob2) { return instance.Poison(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Cancelation, ClientRequest.HearbeatConfirmation, delegate (object ob1, object ob2) { return instance.HearbeatNoticed(ob1, ob2); });;
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.

#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Freeze, ClientRequest.EnrollProject, delegate (object ob1, object ob2) { return instance.RequestUnavailable(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Freeze, ClientRequest.CancelEnrollment, delegate (object ob1, object ob2) { return instance.EnrollmentCanceled(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Freeze, ClientRequest.TaskRequest, delegate (object ob1, object ob2) { return instance.Wait(ob1, ob2); });
            decisionTable.SetAction(ServerPhase.Freeze, ClientRequest.TaskResultsReturnRequest, delegate (object ob1, object ob2) { return instance.Wait(ob1, ob2); });
#pragma warning disable CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.
            decisionTable.SetAction(ServerPhase.Freeze, ClientRequest.HearbeatConfirmation, delegate (object ob1, object ob2) { return instance.HearbeatNoticed(ob1, ob2); });
#pragma warning restore CS8619 // Nullability of reference types in value of type '(ServerResponse, object?)' doesn't match target type '(Enum, object)'.

         
            return decisionTable;
        }

    }
}
