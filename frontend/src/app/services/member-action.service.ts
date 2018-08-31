import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { PagedResult, ODataServiceFactory, ODataService } from './odata';
import { MemberAction } from '../models/member-action';

@Injectable()
export class MemberActionsService {
    readonly odata: ODataService<MemberAction>;

    constructor(private odataFactory: ODataServiceFactory) {
        this.odata = this.odataFactory.CreateService<MemberAction>('MemberActions');
    }

    getMemberActions(event, filterStr = ''): Observable<PagedResult<MemberAction>> {
        let filters = [];
        let query = this.odata
            .Query()
            .Top(event.rows)
            .Skip(event.first);

        if (event.sortField) {
            query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'desc':'asc'));
        } else {
            query.OrderBy('date' + ' ' + (event.sortOrder === 1 ? 'desc':'asc'));
        }

        if (filterStr) {
            let filter = filterStr.trim().toLowerCase();
            let entityFilter = 'contains(tolower(entity),\'' + filter + '\')';
            let memberFilter = 'contains(tolower(memberFullName),\'' + filter + '\')';
            let changesFilter = 'contains(tolower(changedFields),\'' + filter + '\')';
            let objectFilter = 'contains(tolower(changedObject),\'' + filter + '\')';
            let actionFilter = 'contains(tolower(action),\'' + filter + '\')';
            let entityIdFilter = 'contains(tolower(EntityId), \'' + filter + '\')';

            filters.push(entityFilter);
            filters.push(memberFilter);
            filters.push(changesFilter);
            filters.push(actionFilter);
            filters.push(entityIdFilter);
            filters.push(objectFilter);
        }
        query.Filter(filters.join(' or '));
        
        return query.ExecWithCount().map(res => {
            res.data = res.data.map((x: Object) => new MemberAction(x));
            return res;
        });
    }
    
}