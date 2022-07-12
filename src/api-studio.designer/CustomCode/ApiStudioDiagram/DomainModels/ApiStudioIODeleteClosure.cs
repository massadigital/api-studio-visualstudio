﻿// Copyright (c) Andrew Butson.
// Licensed under the MIT License.

using System;

namespace ApiStudioIO
{
    using DslModeling = Microsoft.VisualStudio.Modeling;

    public partial class ApiStudioIODeleteClosure
    {
        public override DslModeling.VisitorFilterResult ShouldVisitRelationship(DslModeling.ElementWalker walker, DslModeling.ModelElement sourceElement,
            DslModeling.DomainRoleInfo sourceRoleInfo, DslModeling.DomainRelationshipInfo domainRelationshipInfo, DslModeling.ElementLink targetRelationship)
        {
            var baseResults = base.ShouldVisitRelationship(walker, sourceElement, sourceRoleInfo, domainRelationshipInfo, targetRelationship);

            //if (sourceElement is HttpApiSuccessResponseModel httpApiSuccessResponseModel)
            //    httpApiSuccessResponseModel.HttpApi.SetDefaults();
            //else if (sourceElement is HttpApiSuccessRequestModel httpApiSuccessRequestModel)
            //    httpApiSuccessRequestModel.HttpApi.SetDefaults();

            return baseResults;
        }

        public override DslModeling.VisitorFilterResult ShouldVisitRolePlayer(DslModeling.ElementWalker walker, DslModeling.ModelElement sourceElement, DslModeling.ElementLink elementLink,
            DslModeling.DomainRoleInfo targetDomainRole, DslModeling.ModelElement targetRolePlayer)
        {
            var baseResults = base.ShouldVisitRolePlayer(walker, sourceElement, elementLink, targetDomainRole, targetRolePlayer);

            //if (sourceElement is HttpApiSuccessResponseModel httpApiSuccessResponseModel)
            //    httpApiSuccessResponseModel.HttpApi.SetDefaults();
            //else if (sourceElement is HttpApiSuccessRequestModel httpApiSuccessRequestModel)
            //    httpApiSuccessRequestModel.HttpApi.SetDefaults();

            return baseResults;
        }
    }
}