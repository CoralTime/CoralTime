import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { ODataServiceFactory, ODataService } from './odata';
import { ProjectRole } from '../models/project-role';

@Injectable()
export class ProjectRolesService {
    readonly odata: ODataService<ProjectRole>;

    constructor(private odataFactory: ODataServiceFactory) {
        this.odata = this.odataFactory.CreateService<ProjectRole>('ProjectRoles');
    }

    getProjectRoles(): Observable<ProjectRole[]> {
        return this.odata.Query().Exec().map((res: any) => {
            return res.map((x: Object) => new ProjectRole(x));
        });
    }
}
